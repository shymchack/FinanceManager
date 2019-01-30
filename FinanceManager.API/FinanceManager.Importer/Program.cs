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

namespace FinanceManager.Importer
{
    class Program
    {

        static void Main(string[] args)
        {

            var filePath = @"D:\Tomek\Wazne\Finanse\Wydatkiwc\trunk\2017.xlsx";
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    // 2. Use the AsDataSet extension method
                    var result = reader.AsDataSet();
                    // The result of each spreadsheet is in result.Tables
                    ParseExcelData(result);
                }
            }
        }

        static void ParseExcelData(DataSet dataSet)
        {
            ConfigModel configModel = LoadConfiguration();

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
                            operation.RepetitionUnitQuantity = 0;
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
                                ReservePeriodQuantity = 1,
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
            var newOperations = new List<MoneyOperation>();
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
                    newMoneyOperation.InitialAmount = initialAmount;
                    newMoneyOperation.OperationSettingID = 1;
                    newMoneyOperation.AccountID = 3;
                    newMoneyOperation.MoneyOperationChanges.AddRange(operationChanges);
                    newOperations.Add(newMoneyOperation);
                    Parallel.ForEach(sameInitialAmountOps, (budOp) =>
                    {
                        budgetedOperations.Remove(budOp);
                    });
                }
            }
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
            operation.OperationSettingID = 1;
            operation.AccountID = 3;

            var operationChange = new MoneyOperationChange();
            operationChange.ChangeAmount = Convert.ToDecimal(rowCollection[thisMonthPayedAmountColumnIndex]);
            operationChange.ChangeDate = DateTime.Parse(sheet.EndDate, CultureInfo.CurrentCulture.DateTimeFormat).AddSeconds(1);
            operation.MoneyOperationChanges.Add(operationChange);
            return operation;
        }

        private static ConfigModel LoadConfiguration()
        {
            var configModel = new ConfigModel();
            // deserialize JSON directly from a file
            using (StreamReader file = File.OpenText(@"D:\Tomek\Dev\Workspace\FinanceManager\FinanceManager.API\FinanceManager.Importer\sampleData.json"))
            {
                var serializer = new JsonSerializer() { ContractResolver = new CamelCasePropertyNamesContractResolver() };
                configModel = (ConfigModel)serializer.Deserialize(file, typeof(ConfigModel));
            }

            return configModel;
        }
    }
}
