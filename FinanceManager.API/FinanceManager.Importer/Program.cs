using ExcelDataReader;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Data;
using System.IO;
using System.Linq;
using FinanceManager.Database.Entities;
using System.Collections.Generic;
using System;
using FinanceManager.Types.Enums;
using System.Globalization;
using System.Threading.Tasks;
using FinanceManager.Database.Context;

namespace FinanceManager.Importer
{
    class Program
    {
        public static Dictionary<string, string> pathToConfigNameDict = new Dictionary<string, string>();
        public static List<MoneyOperation> allMoneyOperations = new List<MoneyOperation>();
        static void Main(string[] args)
        {
            pathToConfigNameDict.Add(@"D:\Tomek\Wazne\Finanse\Wydatkiwc\trunk\2018_values.xlsx", @"D:\Tomek\Dev\Workspace\FinanceManager\FinanceManager.API\FinanceManager.Importer\sampleData2018.json");
            pathToConfigNameDict.Add(@"D:\Tomek\Wazne\Finanse\Wydatkiwc\trunk\2017.xlsx", @"D:\Tomek\Dev\Workspace\FinanceManager\FinanceManager.API\FinanceManager.Importer\sampleData.json");

            foreach(var kvp in pathToConfigNameDict)
            {
                ReadExcelData(kvp.Key, kvp.Value);
            }

            IFinanceManagerContext context = new FinanceManagerContext();
            foreach(var monOp in allMoneyOperations)
            {
                context.MoneyOperations.Add(monOp);
            }
            var oko = allMoneyOperations.Where(m => m.ValidityBeginDate.Year < 2000 || m.ValidityEndDate.Year < 2000 || m.NextOperationExecutionDate.Year < 2000);
            var oko2 = allMoneyOperations.Where(m => m.MoneyOperationChanges.Any(mo => mo.ChangeDate.Year < 2000));
            context.SaveChanges();
        }

        private static void ReadExcelData(string dataFilePath, string configFilePath)
        {
            using (var stream = File.Open(dataFilePath, FileMode.Open, FileAccess.Read))
            {
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    // 2. Use the AsDataSet extension method
                    var result = reader.AsDataSet();
                    // The result of each spreadsheet is in result.Tables
                    ParseExcelData(result, configFilePath);
                }
            }
        }

        static void ParseExcelData(DataSet dataSet, string configFilePath)
        {
            ConfigModel configModel = LoadConfiguration(configFilePath);

            List<MoneyOperation> singleOperations = new List<MoneyOperation>();
            List<MoneyOperation> cyclicOperations = new List<MoneyOperation>();
            List<MoneyOperation> budgetedOperations = new List<MoneyOperation>();

            string errors = string.Empty;

            foreach (Sheet sheet in configModel.ExcelFolder.Sheets)
            {
                var tableCollection = dataSet.Tables[sheet.Name];
                var currentMonthRowIndex = sheet.CurrentMonthRowIndex;
                var currentMonthColumnIndex = sheet.CurrentMonthColumnIndex;
                var currentMonth = Int32.Parse(tableCollection.Rows[currentMonthRowIndex][currentMonthColumnIndex].ToString());
                foreach (var oko in sheet.SingleCommitments)
                {
                    for (int i = oko.FirstRowIndex; i <= oko.LastRowIndex; i++)
                    {
                        var rowCollection = tableCollection.Rows[i];
                        try
                        {
                            var operation = ReadCommitmentData(sheet, oko, rowCollection);
                            operation.RepetitionUnit = PeriodUnit.Month;
                            operation.RepetitionUnitQuantity = 0;
                            operation.OperationSetting = null;
                            singleOperations.Add(operation);
                        }
                        catch (Exception ex)
                        {
                            errors += ReportError(sheet, oko, rowCollection, $"\tSINGLE({i})\t" + ex.Message);
                        }
                    }
                }
                foreach (var oko in sheet.BudgetedCommitments)
                {
                    for (int i = oko.FirstRowIndex; i <= oko.LastRowIndex; i++)
                    {
                        var rowCollection = tableCollection.Rows[i];
                        try
                        {
                            var operation = ReadCommitmentData(sheet, oko, rowCollection);
                            operation.RepetitionUnit = PeriodUnit.Month;
                            operation.RepetitionUnitQuantity = 1;
                            operation.OperationSetting = new MoneyOperationSetting()
                            {
                                ReservePeriodQuantity = Convert.ToInt32((double)rowCollection[oko.PaymentMonthNumberColumnIndex]) - currentMonth,
                                ReservePeriodUnit = PeriodUnit.Month
                            };
                            budgetedOperations.Add(operation);
                        }
                        catch (Exception ex)
                        {
                            errors += ReportError(sheet, oko, rowCollection, $"\tBUDGETED({i})\t" + ex.Message);
                        }
                    }
                }
                foreach (var oko in sheet.CyclicCommitments)
                {
                    for (int i = oko.FirstRowIndex; i <= oko.LastRowIndex; i++)
                    {
                        var rowCollection = tableCollection.Rows[i];
                        try
                        {
                            var operation = ReadCommitmentData(sheet, oko, rowCollection);
                            operation.RepetitionUnit = PeriodUnit.Month;
                            operation.RepetitionUnitQuantity = 1;
                            operation.OperationSetting = new MoneyOperationSetting()
                            {
                                ReservePeriodQuantity = 0,
                                ReservePeriodUnit = PeriodUnit.Month
                            };
                            cyclicOperations.Add(operation);
                        }
                        catch (Exception ex)
                        {
                            errors += ReportError(sheet, oko, rowCollection, $"\tCYCLIC({i})\t" + ex.Message);
                        }
                    }
                }
                foreach (var oko in sheet.Incomes)
                {
                    for (int i = oko.FirstRowIndex; i <= oko.LastRowIndex; i++)
                    {
                        var rowCollection = tableCollection.Rows[i];
                        try
                        {
                            var operation = ReadCommitmentData(sheet, oko, rowCollection);
                            operation.InitialAmount = -operation.InitialAmount;
                            operation.RepetitionUnit = PeriodUnit.Month;
                            operation.RepetitionUnitQuantity = 0;
                            singleOperations.Add(operation);
                        }
                        catch (Exception ex)
                        {
                            errors += ReportError(sheet, oko, rowCollection, $"\tINCOMES({i})\t"+ex.Message);
                        }
                    }
                }
            }
            var newBudgetedOperations = new List<MoneyOperation>();
            foreach(var budOps in budgetedOperations.GroupBy(bo => bo.Name))
            {
                foreach(var sameInitialAmountOps in budOps.GroupBy(bo => bo.InitialAmount))
                {
                    var validityEndDate = sameInitialAmountOps.OrderByDescending(op => op.ValidityEndDate).First().ValidityEndDate;
                    var validityBeginDate = sameInitialAmountOps.OrderBy(op => op.ValidityBeginDate).First().ValidityBeginDate;
                    var commonOperationDataSource = sameInitialAmountOps.First();
                    var name = commonOperationDataSource.Name;
                    var initialAmount = commonOperationDataSource.InitialAmount;
                    var operationChanges = sameInitialAmountOps.SelectMany(o => o.MoneyOperationChanges).Where(oc => oc.ChangeDate <= validityEndDate && oc.ChangeDate >= validityBeginDate);
                    var newMoneyOperation = new MoneyOperation();
                    newMoneyOperation.Name = name;
                    newMoneyOperation.IsReal = true;
                    newMoneyOperation.IsActive = true;
                    newMoneyOperation.ValidityBeginDate = validityBeginDate;
                    newMoneyOperation.ValidityEndDate = validityEndDate;
                    newMoneyOperation.NextOperationExecutionDate = commonOperationDataSource.NextOperationExecutionDate;
                    newMoneyOperation.InitialAmount = initialAmount;
                    newMoneyOperation.OperationSetting = new MoneyOperationSetting()
                    {
                        ReservePeriodQuantity = commonOperationDataSource.OperationSetting.ReservePeriodQuantity,
                        ReservePeriodUnit = PeriodUnit.Month
                    };
                    newMoneyOperation.AccountID = 3;
                    newMoneyOperation.RepetitionUnit = commonOperationDataSource.RepetitionUnit;
                    newMoneyOperation.RepetitionUnitQuantity = commonOperationDataSource.RepetitionUnitQuantity;
                    newMoneyOperation.MoneyOperationChanges.AddRange(operationChanges);
                    newBudgetedOperations.Add(newMoneyOperation);
                    Parallel.ForEach(sameInitialAmountOps, (budOp) =>
                    {
                        budgetedOperations.Remove(budOp);
                    });
                }
            }
            var newCyclicOperations = new List<MoneyOperation>();
            foreach (var cycOps in cyclicOperations.GroupBy(bo => bo.Name))
            {
                foreach (var sameInitialAmountOps in cycOps.GroupBy(bo => bo.InitialAmount))
                {
                    var validityEndDate = sameInitialAmountOps.OrderByDescending(op => op.ValidityEndDate).First().ValidityEndDate;
                    var validityBeginDate = sameInitialAmountOps.OrderBy(op => op.ValidityBeginDate).First().ValidityBeginDate;
                    var commonOperationDataSource = sameInitialAmountOps.First();
                    var name = commonOperationDataSource.Name;
                    var initialAmount = commonOperationDataSource.InitialAmount;
                    var operationChanges = sameInitialAmountOps.SelectMany(o => o.MoneyOperationChanges).Where(oc => oc.ChangeDate <= validityEndDate && oc.ChangeDate >= validityBeginDate);
                    var newMoneyOperation = new MoneyOperation();
                    newMoneyOperation.Name = name;
                    newMoneyOperation.IsReal = true;
                    newMoneyOperation.IsActive = true;
                    newMoneyOperation.ValidityBeginDate = validityBeginDate;
                    newMoneyOperation.ValidityEndDate = validityEndDate;
                    newMoneyOperation.NextOperationExecutionDate = commonOperationDataSource.NextOperationExecutionDate;
                    newMoneyOperation.InitialAmount = initialAmount;
                    newMoneyOperation.AccountID = 3;
                    newMoneyOperation.OperationSetting = new MoneyOperationSetting()
                    {
                        ReservePeriodQuantity = 0,
                        ReservePeriodUnit = PeriodUnit.Month
                    };
                    newMoneyOperation.MoneyOperationChanges.AddRange(operationChanges);
                    newCyclicOperations.Add(newMoneyOperation);
                    Parallel.ForEach(sameInitialAmountOps, (budOp) =>
                    {
                        cyclicOperations.Remove(budOp);
                    });
                }
            }
            allMoneyOperations.AddRange(singleOperations.Concat(newCyclicOperations).Concat(newBudgetedOperations));
        }

        private static string ReportError(Sheet sheet, Commitment oko, DataRow rowCollection, string message)
        {
            return $"{sheet.Name}\t row {oko.FirstRowIndex} to {oko.LastRowIndex}\t column index {oko.NameColumnIndex}: {rowCollection[oko.NameColumnIndex]}\t {message}\n";
        }

        private static MoneyOperation ReadCommitmentData(Sheet sheet, Commitment commitment, DataRow rowCollection)
        {
            var totalAmountColumnIndex = commitment.TotalAmountColumnIndex;
            var thisMonthPayedAmountColumnIndex = commitment.ThisMonthPayedAmountColumnIndex;
            var nameColumnIndex = commitment.NameColumnIndex;
            var operation = new MoneyOperation();
            operation.IsReal = true;
            operation.IsActive = true;
            operation.ValidityBeginDate = DateTime.Parse(sheet.BeginDate, CultureInfo.CurrentCulture.DateTimeFormat);
            operation.ValidityEndDate = DateTime.Parse(sheet.EndDate, CultureInfo.CurrentCulture.DateTimeFormat);
            operation.InitialAmount = Convert.ToDecimal(rowCollection[totalAmountColumnIndex]);
            operation.Name = (string)rowCollection[nameColumnIndex];
            operation.AccountID = 3;
            operation.NextOperationExecutionDate = operation.ValidityEndDate;
            var operationChange = new MoneyOperationChange();
            operationChange.ChangeAmount = -Convert.ToDecimal(rowCollection[thisMonthPayedAmountColumnIndex]);
            operationChange.ChangeDate = DateTime.Parse(sheet.BeginDate, CultureInfo.CurrentCulture.DateTimeFormat).AddSeconds(1);
            operation.MoneyOperationChanges.Add(operationChange);
            return operation;
        }

        private static ConfigModel LoadConfiguration(string configFilePath)
        {
            var configModel = new ConfigModel();
            // deserialize JSON directly from a file
            using (StreamReader file = File.OpenText(configFilePath))
            {
                var serializer = new JsonSerializer() { ContractResolver = new CamelCasePropertyNamesContractResolver() };
                configModel = (ConfigModel)serializer.Deserialize(file, typeof(ConfigModel));
            }

            return configModel;
        }
    }
}
