using System;
using NUnit.Framework;
using FinanceManager.API.Services;
using FinanceManager.DAL.Repositories;
using FinanceManager.DAL.Repositories.Contracts;
using FinanceManager.Database.Context;
using FinanceManager.DAL.UnitOfWork;
using FinanceManager.Types.Enums;
using FinanceManager.BL;
using FinanceManager.Database.Entities;
using System.Linq;

namespace FinanceManager.API.Tests.Services
{
    [TestFixture]
    public class MoneyOperationsServiceTest
    {

        private IMoneyOperationsService _moneyOperationsService => new MoneyOperationsService(_moOpUOW, _moOpLogic);
        private IMoneyOperationsRepository _moOpRepo => new MoneyOperationsRepository(_finManContext);
        private IAccountsRepository _accRepo => new AccountsRepository(_finManContext);
        private IPeriodicityLogic _periodicityLogic = new PeriodicityLogic();
        private IMoneyOperationLogic _moOpLogic => new MoneyOperationLogic(_periodicityLogic);
        private IMoneyOperationsUnitOfWork _moOpUOW => new MoneyOperationsUnitOfWork(null, _moOpRepo, _accRepo);
        private IFinanceManagerContext _finManContext;

        public MoneyOperationsServiceTest()
        {
        }

        //public void AddMoneyOperationTest()
        //{
        //    var context = new FakeFinanceManagerContext();

        //    SetContext(context);

        //    MoneyOperationModel data = new MoneyOperationModel();
        //    data.AccountID = new Random().Next();
        //    data.Description = "Some test description";
        //    data.InitialAmount = new Random().Next();
        //    data.IsActive = true;
        //    data.IsReal = true;
        //    data.Name = "A test money operation";
        //    data.RepetitionUnit = PeriodUnit.Month;
        //    data.RepetitionUnitQuantity = 1;
        //    data.ValidityBeginDate = DateTime.UtcNow.AddDays(-10);
        //    data.ValidityEndDate = DateTime.UtcNow.AddMonths(2);

        //    int id = _moneyOperationsService.AddMoneyOperation(data);

        //    Assert.GreaterOrEqual(id, 0);
        //}

        #region operations that doesn't reach any total month value
        [Test]
        public void GetMoneyOperation_One_Cyclic_NoOperationChanges()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = testDate.AddMonths(-1);
            moneyOperation.ValidityEndDate = testDate.AddMonths(1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 0
            };
            moneyOperation.OperationSettingID = 1;

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operation = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operation.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operation);
            Assert.AreEqual(40*3, operation.InitialAmount);
            Assert.AreEqual(120, operation.TotalAmount);
            Assert.AreEqual(80, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(80, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(80, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(0, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Cyclic_OneOperationChangeInPastPeriod()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = testDate.AddMonths(-1);
            moneyOperation.ValidityEndDate = testDate.AddMonths(1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 0
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -30,
                ChangeDate = moneyOperation.ValidityBeginDate.AddDays(1),
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40*3, operationSchedule.InitialAmount);
            Assert.AreEqual(90, operationSchedule.TotalAmount);
            Assert.AreEqual(50, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(50, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(50, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount); //0 because it is not budgeted operation so only current period gets budgeted
            Assert.AreEqual(0, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Cyclic_OneOperationChangeInCurrentPeriod()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = testDate.AddMonths(-1);
            moneyOperation.ValidityEndDate = testDate.AddMonths(1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 0
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -30,
                ChangeDate = testDate,
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40 * 3, operationSchedule.InitialAmount);
            Assert.AreEqual(90, operationSchedule.TotalAmount);
            Assert.AreEqual(50, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(50, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(50, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(-30, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Cyclic_OneOperationChangeInFuturePeriod()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = FirstSecondOfPreviousMonth(testDate);
            moneyOperation.ValidityEndDate = LastSecondOfNextMonth(testDate);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 0
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -30,
                ChangeDate = testDate.AddMonths(1),
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40 * 3, operationSchedule.InitialAmount);
            Assert.AreEqual(120, operationSchedule.TotalAmount);
            Assert.AreEqual(80, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(80, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(80, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(0, scheduleItem.CurrentChangeAmount);
        }

        private DateTime FirstSecondOfPreviousMonth(DateTime testDate)
        {
            var previousMonthDate = testDate.AddMonths(-1);
            return new DateTime(previousMonthDate.Year, previousMonthDate.Month, 1);
        }

        private DateTime LastSecondOfNextMonth(DateTime testDate)
        {
            var nextMonthDate = testDate.AddMonths(1);
            return new DateTime(nextMonthDate.Year, nextMonthDate.Month, DateTime.DaysInMonth(nextMonthDate.Year, nextMonthDate.Month),23,59,59);
        }

        [Test]
        public void GetMoneyOperation_One_Cyclic_TwoOperationChangesInPastAndCurrentPeriod()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = testDate.AddMonths(-1);
            moneyOperation.ValidityEndDate = testDate.AddMonths(1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 0
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -30,
                ChangeDate = testDate,
                MoneyOperationID = moneyOperation.ID
            });
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 2,
                ChangeAmount = -20,
                ChangeDate = moneyOperation.ValidityBeginDate.AddDays(1),
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40 * 3, operationSchedule.InitialAmount);
            Assert.AreEqual(70, operationSchedule.TotalAmount);
            Assert.AreEqual(30, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(30, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(30, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(-30, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Cyclic_TwoOperationChangesInEachPastAndCurrentPeriod()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = testDate.AddMonths(-1);
            moneyOperation.ValidityEndDate = testDate.AddMonths(1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 0
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -30,
                ChangeDate = testDate,
                MoneyOperationID = moneyOperation.ID
            });
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 2,
                ChangeAmount = -20,
                ChangeDate = moneyOperation.ValidityBeginDate.AddDays(1),
                MoneyOperationID = moneyOperation.ID
            });
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 3,
                ChangeAmount = -7,
                ChangeDate = testDate.AddDays(-1),
                MoneyOperationID = moneyOperation.ID
            });
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 4,
                ChangeAmount = -15,
                ChangeDate = moneyOperation.ValidityBeginDate.AddDays(2),
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40 * 3, operationSchedule.InitialAmount);
            Assert.AreEqual(48, operationSchedule.TotalAmount);
            Assert.AreEqual(8, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(8, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(8, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(-37, scheduleItem.CurrentChangeAmount);
        }

        #endregion

        #region Operations that have money operation changes equal to every month value
        [Test]
        public void GetMoneyOperation_One_Cyclic_OneOperationChangeInPastPeriod_EqualsMonthValue()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = testDate.AddMonths(-1);
            moneyOperation.ValidityEndDate = testDate.AddMonths(1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 0
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -40,
                ChangeDate = moneyOperation.ValidityBeginDate.AddDays(1),
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40 * 3, operationSchedule.InitialAmount);
            Assert.AreEqual(80, operationSchedule.TotalAmount);
            Assert.AreEqual(40, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(40, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(40, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount); //0 because it is not budgeted operation so only current period gets budgeted
            Assert.AreEqual(0, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Cyclic_OneOperationChangeInCurrentPeriod_EqualsMonthValue()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = testDate.AddMonths(-1);
            moneyOperation.ValidityEndDate = testDate.AddMonths(1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 0
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -40,
                ChangeDate = testDate,
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40 * 3, operationSchedule.InitialAmount);
            Assert.AreEqual(80, operationSchedule.TotalAmount);
            Assert.AreEqual(40, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(40, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(40, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(-40, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Cyclic_OneOperationChangeInFuturePeriod_EqualsMonthValue()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = FirstSecondOfPreviousMonth(testDate);
            moneyOperation.ValidityEndDate = LastSecondOfNextMonth(testDate);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 0
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -40,
                ChangeDate = testDate.AddMonths(1),
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40 * 3, operationSchedule.InitialAmount);
            Assert.AreEqual(120, operationSchedule.TotalAmount);
            Assert.AreEqual(80, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(80, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(80, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(0, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Cyclic_TwoOperationChangesInPastAndCurrentPeriod_EqualsMonthValue()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = testDate.AddMonths(-1);
            moneyOperation.ValidityEndDate = testDate.AddMonths(1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 0
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -40,
                ChangeDate = testDate,
                MoneyOperationID = moneyOperation.ID
            });
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 2,
                ChangeAmount = -40,
                ChangeDate = moneyOperation.ValidityBeginDate.AddDays(1),
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40 * 3, operationSchedule.InitialAmount);
            Assert.AreEqual(40, operationSchedule.TotalAmount);
            Assert.AreEqual(0, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(0, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(0, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(-40, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Cyclic_TwoOperationChangesInEachPastAndCurrentPeriod_EqualsMonthValue()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = testDate.AddMonths(-1);
            moneyOperation.ValidityEndDate = testDate.AddMonths(1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 0
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -15,
                ChangeDate = testDate,
                MoneyOperationID = moneyOperation.ID
            });
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 2,
                ChangeAmount = -27,
                ChangeDate = moneyOperation.ValidityBeginDate.AddDays(1),
                MoneyOperationID = moneyOperation.ID
            });
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 3,
                ChangeAmount = -25,
                ChangeDate = testDate.AddDays(-1),
                MoneyOperationID = moneyOperation.ID
            });
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 4,
                ChangeAmount = -13,
                ChangeDate = moneyOperation.ValidityBeginDate.AddDays(2),
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40 * 3, operationSchedule.InitialAmount);
            Assert.AreEqual(40, operationSchedule.TotalAmount);
            Assert.AreEqual(0, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(0, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(0, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(-40, scheduleItem.CurrentChangeAmount);
        }
        #endregion

        #region Operations that exceeds money operation month value
        [Test]
        public void GetMoneyOperation_One_Cyclic_OneOperationChangeInPastPeriod_ExceedsMonthValue()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = testDate.AddMonths(-1);
            moneyOperation.ValidityEndDate = testDate.AddMonths(1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 0
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -50,
                ChangeDate = moneyOperation.ValidityBeginDate.AddDays(1),
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40 * 3, operationSchedule.InitialAmount);
            Assert.AreEqual(70, operationSchedule.TotalAmount);
            Assert.AreEqual(30, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(30, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(30, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount); //0 because it is not budgeted operation so only current period gets budgeted
            Assert.AreEqual(0, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Cyclic_OneOperationChangeInCurrentPeriod_ExceedsMonthValue()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = testDate.AddMonths(-1);
            moneyOperation.ValidityEndDate = testDate.AddMonths(1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 0
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -50,
                ChangeDate = testDate,
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40 * 3, operationSchedule.InitialAmount);
            Assert.AreEqual(70, operationSchedule.TotalAmount);
            Assert.AreEqual(30, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(30, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(30, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(-50, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Cyclic_OneOperationChangeInFuturePeriod_ExceedsMonthValue()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = FirstSecondOfPreviousMonth(testDate);
            moneyOperation.ValidityEndDate = LastSecondOfNextMonth(testDate);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 0
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -50,
                ChangeDate = testDate.AddMonths(1),
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40 * 3, operationSchedule.InitialAmount);
            Assert.AreEqual(120, operationSchedule.TotalAmount);
            Assert.AreEqual(80, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(80, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(80, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(0, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Cyclic_TwoOperationChangesInPastAndCurrentPeriod_PastExceedsMonthValue()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = testDate.AddMonths(-1);
            moneyOperation.ValidityEndDate = testDate.AddMonths(1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 0
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -20,
                ChangeDate = testDate,
                MoneyOperationID = moneyOperation.ID
            });
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 2,
                ChangeAmount = -50,
                ChangeDate = moneyOperation.ValidityBeginDate.AddDays(1),
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40 * 3, operationSchedule.InitialAmount);
            Assert.AreEqual(50, operationSchedule.TotalAmount);
            Assert.AreEqual(10, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(10, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(10, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(-20, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Cyclic_TwoOperationChangesInPastAndCurrentPeriod_CurrentExceedsMonthValue()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = testDate.AddMonths(-1);
            moneyOperation.ValidityEndDate = testDate.AddMonths(1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 0
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -50,
                ChangeDate = testDate,
                MoneyOperationID = moneyOperation.ID
            });
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 2,
                ChangeAmount = -20,
                ChangeDate = moneyOperation.ValidityBeginDate.AddDays(1),
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40 * 3, operationSchedule.InitialAmount);
            Assert.AreEqual(50, operationSchedule.TotalAmount);
            Assert.AreEqual(10, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(10, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(10, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(-50, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Cyclic_TwoOperationChangesInEachPastAndCurrentPeriod_ExceedsMonthValue()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = testDate.AddMonths(-1);
            moneyOperation.ValidityEndDate = testDate.AddMonths(1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 0
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -27,
                ChangeDate = testDate,
                MoneyOperationID = moneyOperation.ID
            });
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 2,
                ChangeAmount = -28,
                ChangeDate = moneyOperation.ValidityBeginDate.AddDays(1),
                MoneyOperationID = moneyOperation.ID
            });
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 3,
                ChangeAmount = -23,
                ChangeDate = testDate.AddDays(-1),
                MoneyOperationID = moneyOperation.ID
            });
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 4,
                ChangeAmount = -21,
                ChangeDate = moneyOperation.ValidityBeginDate.AddDays(2),
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40 * 3, operationSchedule.InitialAmount);
            Assert.AreEqual(21, operationSchedule.TotalAmount);
            Assert.AreEqual(0, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(0, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(-19, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(-50, scheduleItem.CurrentChangeAmount);
        }
        #endregion
        
        [Test]
        public void GetMoneyOperation_One_Budgeted_NoOperationChanges_OneMonthDuration_Past()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = new DateTime(testDate.Year, testDate.Month-1, 1);
            moneyOperation.ValidityEndDate = FirstSecondOfCurrentMonth(testDate).AddSeconds(-1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 1,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(40, operationSchedule.TotalAmount);
            Assert.AreEqual(40, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(40, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(40, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount); //0 because it is not budgeted operation so only current period gets budgeted
            Assert.AreEqual(0, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Budgeted_NoOperationChanges_OneMonthDuration_PastPlusOneDay()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = new DateTime(testDate.Year, testDate.Month -1, 1);
            moneyOperation.ValidityEndDate = FirstSecondOfCurrentMonth(testDate);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 1,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(40, operationSchedule.TotalAmount);
            Assert.AreEqual(40, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(40, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(40, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount); //0 because it is not budgeted operation so only current period gets budgeted
            Assert.AreEqual(0, scheduleItem.CurrentChangeAmount);
        }
        public void GetMoneyOperation_One_Budgeted_NoOperationChanges_OneMonthDuration_PastMinusOneDay()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = new DateTime(testDate.Year, testDate.Month - 2, 1).AddSeconds(-1);
            moneyOperation.ValidityEndDate = FirstSecondOfCurrentMonth(testDate).AddSeconds(-1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 1,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(40, operationSchedule.TotalAmount);
            Assert.AreEqual(40, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(40, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(40, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount); //0 because it is not budgeted operation so only current period gets budgeted
            Assert.AreEqual(0, scheduleItem.CurrentChangeAmount);
        }
        [Test]
        public void GetMoneyOperation_One_Budgeted_NoOperationChanges_OneMonthDuration_Current()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = FirstSecondOfCurrentMonth(testDate);
            moneyOperation.ValidityEndDate = new DateTime(testDate.Year, testDate.Month+1, 1).AddSeconds(-1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 1,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(40, operationSchedule.TotalAmount);
            Assert.AreEqual(40, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(40, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(40, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(0, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Budgeted_NoOperationChanges_OneMonthDuration_CurrentPlusOneDay()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = FirstSecondOfCurrentMonth(testDate);
            moneyOperation.ValidityEndDate = new DateTime(testDate.Year, testDate.Month + 1, 1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 1,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(40, operationSchedule.TotalAmount);
            Assert.AreEqual(40, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(20, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(40, scheduleItem.TotalAmount);
            Assert.AreEqual(20, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(0, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Budgeted_NoOperationChanges_OneMonthDuration_CurrentMinusOneDay()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = FirstSecondOfCurrentMonth(testDate).AddSeconds(-1);
            moneyOperation.ValidityEndDate = new DateTime(testDate.Year, testDate.Month + 1, 1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 1,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(40, operationSchedule.TotalAmount);
            Assert.AreEqual(40, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(40, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(40, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(0, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Budgeted_NoOperationChanges_OneMonthDuration_Future()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = FirstSecondOfNextMonth(testDate);
            moneyOperation.ValidityEndDate = LastSecondOfNextMonth(testDate);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 1,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(40, operationSchedule.TotalAmount);
            Assert.IsNull(scheduleItem);
        }

        private DateTime FirstSecondOfNextMonth(DateTime testDate)
        {
            return testDate.AddMonths(1).AddDays(-testDate.Day+1).AddTicks(-testDate.TimeOfDay.Ticks);
        }

        [Test]
        public void GetMoneyOperation_One_Budgeted_NoOperationChanges_OneMonthDuration_StartedInFutureMonth()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = FirstSecondOfNextMonth(testDate);
            moneyOperation.ValidityEndDate = LastSecondOfNextMonth(testDate);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 1,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(40, operationSchedule.TotalAmount);
            Assert.IsNull(scheduleItem);
        }

        [Test]
        public void GetMoneyOperation_One_Budgeted_OneOperationChangeInPast_OneMonthDuration_Past()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = new DateTime(testDate.Year, testDate.Month -1, 1);
            moneyOperation.ValidityEndDate = FirstSecondOfCurrentMonth(testDate).AddSeconds(-1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 1,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -30,
                ChangeDate = moneyOperation.ValidityBeginDate.AddDays(1),
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(10, operationSchedule.TotalAmount);
            Assert.AreEqual(10, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(10, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(10, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(0, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Budgeted_OneOperationChangeInPast_OneMonthDuration_Current()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = FirstSecondOfCurrentMonth(testDate);
            moneyOperation.ValidityEndDate = new DateTime(testDate.Year, testDate.Month + 1, 1).AddSeconds(-1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 1,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -30,
                ChangeDate = moneyOperation.ValidityBeginDate.AddDays(-1),
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(10, operationSchedule.TotalAmount);
            Assert.AreEqual(10, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(10, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(10, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(0, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Budgeted_OneOperationChangeInPast_OneMonthDuration_Future()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = FirstSecondOfNextMonth(testDate);
            moneyOperation.ValidityEndDate = LastSecondOfNextMonth(testDate);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 1,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -30,
                ChangeDate = testDate.AddDays(-1),
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(10, operationSchedule.TotalAmount);
            Assert.IsNull(scheduleItem);
        }

        [Test]
        public void GetMoneyOperation_One_Budgeted_OneOperationChangeInCurrent_OneMonthDuration_Past()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = new DateTime(testDate.Year, testDate.Month - 1, 1);
            moneyOperation.ValidityEndDate = FirstSecondOfCurrentMonth(testDate).AddSeconds(-1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 1,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -10,
                ChangeDate = testDate.AddMonths(-1),
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(30, operationSchedule.TotalAmount);
            Assert.AreEqual(30, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(30, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(30, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(0, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Budgeted_OneOperationChangeInCurrent_OneMonthDuration_Current()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = FirstSecondOfCurrentMonth(testDate);
            moneyOperation.ValidityEndDate = new DateTime(testDate.Year, testDate.Month + 1, 1).AddSeconds(-1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 1,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -10,
                ChangeDate = testDate,
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(30, operationSchedule.TotalAmount);
            Assert.AreEqual(30, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(30, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(30, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(-10, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Budgeted_OneOperationChangeInCurrent_OneMonthDuration_Future()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = FirstSecondOfNextMonth(testDate);
            moneyOperation.ValidityEndDate = LastSecondOfNextMonth(testDate);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 1,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -10,
                ChangeDate = testDate,
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(30, operationSchedule.TotalAmount);
            Assert.IsNull(scheduleItem);
        }

        [Test]
        public void GetMoneyOperation_One_Budgeted_OneOperationChangeInFuture_OneMonthDuration_Past()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = new DateTime(testDate.Year, testDate.Month - 1, 1);
            moneyOperation.ValidityEndDate = FirstSecondOfCurrentMonth(testDate).AddSeconds(-1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 1,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -10,
                ChangeDate = testDate.AddMonths(1),
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(40, operationSchedule.TotalAmount);
            Assert.AreEqual(40, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(40, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(40, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(0, scheduleItem.CurrentChangeAmount);
        }

        public void GetMoneyOperation_One_Budgeted_OneOperationChangeInFuture_OneMonthDuration_Current()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = FirstSecondOfCurrentMonth(testDate);
            moneyOperation.ValidityEndDate = new DateTime(testDate.Year, testDate.Month + 1, 1).AddSeconds(-1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 1,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -10,
                ChangeDate = testDate,
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(30, operationSchedule.TotalAmount);
            Assert.AreEqual(30, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(30, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(30, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(-10, scheduleItem.CurrentChangeAmount);
        }

        public void GetMoneyOperation_One_Budgeted_OneOperationChangeInFuture_OneMonthDuration_Future()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = new DateTime(testDate.Year, testDate.Month + 1, 1);
            moneyOperation.ValidityEndDate = new DateTime(testDate.Year, testDate.Month + 2, 1).AddSeconds(-1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 1,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -10,
                ChangeDate = testDate,
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(30, operationSchedule.TotalAmount);
            Assert.IsNull(scheduleItem);
        }
        [Test]
        public void GetMoneyOperation_One_Budgeted_NoOperationChanges_MultipleMonthsDuration_AlreadyFinishedInPastMonth()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = new DateTime(testDate.Year, testDate.AddMonths(-4).Month, 1);
            moneyOperation.ValidityEndDate = FirstSecondOfCurrentMonth(testDate).AddSeconds(-1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 4,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(40, operationSchedule.TotalAmount);
            Assert.AreEqual(40, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(40, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(40, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(0, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Budgeted_NoOperationChanges_MultipleMonthsDuration_GettingFinishedInCurrentMonth()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = new DateTime(testDate.Year, testDate.Month - 3, 1);
            moneyOperation.ValidityEndDate = new DateTime(testDate.Year, testDate.Month + 1, 1).AddSeconds(-1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 4,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(40, operationSchedule.TotalAmount);
            Assert.AreEqual(40, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(40, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(40, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(0, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Budgeted_NoOperationChanges_MultipleMonthsDuration_StartedInCurrentMonth()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = FirstSecondOfCurrentMonth(testDate);
            moneyOperation.ValidityEndDate = LastSecondOf3rdMonthFromCurrent(testDate);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 4,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(40, operationSchedule.TotalAmount);
            Assert.AreEqual(40, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(10, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(40, scheduleItem.TotalAmount);
            Assert.AreEqual(30, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(0, scheduleItem.CurrentChangeAmount);
        }

        private DateTime LastSecondOf3rdMonthFromCurrent(DateTime testDate)
        {
            var nextMonthDate = testDate.AddMonths(3);
            return new DateTime(nextMonthDate.Year, nextMonthDate.Month, DateTime.DaysInMonth(nextMonthDate.Year, nextMonthDate.Month), 23, 59, 59);
        }

        private DateTime FirstSecondOfCurrentMonth(DateTime testDate)
        {
            return testDate.AddDays(-testDate.Day + 1).AddTicks(-testDate.TimeOfDay.Ticks);
        }

        [Test]
        public void GetMoneyOperation_One_Budgeted_OneOperationChangeInPast_MultipleMonthsDuration_AlreadyFinishedInPastMonth()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = FirstSecondOf4thMonthPastFromCurrent(testDate);
            moneyOperation.ValidityEndDate = FirstSecondOfCurrentMonth(testDate).AddSeconds(-1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 4,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -30,
                ChangeDate = moneyOperation.ValidityBeginDate.AddDays(1),
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(10, operationSchedule.TotalAmount);
            Assert.AreEqual(10, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(10, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(10, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(0, scheduleItem.CurrentChangeAmount);
        }

        private DateTime FirstSecondOf4thMonthPastFromCurrent(DateTime testDate)
        {
            return testDate.AddMonths(-4).AddDays(-testDate.Day + 1).AddTicks(-testDate.TimeOfDay.Ticks);
        }

        [Test]
        public void GetMoneyOperation_One_Budgeted_OneOperationChangeInPast_MultipleMonthsDuration_GettingFinishedInCurrentMonth()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = new DateTime(testDate.Year, testDate.Month - 3, 1);
            moneyOperation.ValidityEndDate = new DateTime(testDate.Year, testDate.Month + 1, 1).AddSeconds(-1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 4,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -30,
                ChangeDate = moneyOperation.ValidityBeginDate.AddDays(1),
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(10, operationSchedule.TotalAmount);
            Assert.AreEqual(10, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(10, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(10, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(0, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Budgeted_OneOperationChangeInPast_MultipleMonthsDuration_StartedInCurrentMonth()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = FirstSecondOfCurrentMonth(testDate);
            moneyOperation.ValidityEndDate = LastSecondOf3rdMonthFromCurrent(testDate);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 4,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -10,
                ChangeDate = moneyOperation.ValidityBeginDate.AddDays(1),
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(30, operationSchedule.TotalAmount);
            Assert.AreEqual(30, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(0, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(30, scheduleItem.TotalAmount);
            Assert.AreEqual(30, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(-10, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Budgeted_OneOperationChangeInPast_OneMonthDuration_StartedInFutureMonth()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = FirstSecondOfNextMonth(testDate);
            moneyOperation.ValidityEndDate = LastSecondOfNextMonth(testDate);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 1,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -10,
                ChangeDate = testDate.AddMonths(-1),
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(30, operationSchedule.TotalAmount);
            Assert.IsNull(scheduleItem);
        }

        [Test]
        public void GetMoneyOperation_One_Budgeted_OneOperationChangeInCurrent_MultipleMonthsDuration_AlreadyFinishedInPastMonth()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = new DateTime(testDate.Year, testDate.Month-4, 1);
            moneyOperation.ValidityEndDate = FirstSecondOfCurrentMonth(testDate).AddSeconds(-1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 4,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -10,
                ChangeDate = testDate,
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(30, operationSchedule.TotalAmount);
            Assert.AreEqual(30, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(30, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(30, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(-10, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Budgeted_OneOperationChangeInCurrent_MultipleMonthsDuration_GettingFinishedInCurrentMonth()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = new DateTime(testDate.Year, testDate.Month - 3, 1);
            moneyOperation.ValidityEndDate = new DateTime(testDate.Year, testDate.Month + 1, 1).AddSeconds(-1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 4,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -10,
                ChangeDate = testDate,
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(30, operationSchedule.TotalAmount);
            Assert.AreEqual(30, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(30, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(30, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(-10, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Budgeted_OneOperationChangeInCurrent_MultipleMonthsDuration_StartedInCurrentMonth()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = FirstSecondOfCurrentMonth(testDate);
            moneyOperation.ValidityEndDate = LastSecondOf3rdMonthFromCurrent(testDate);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 4,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -10,
                ChangeDate = testDate,
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(30, operationSchedule.TotalAmount);
            Assert.AreEqual(30, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(0, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(30, scheduleItem.TotalAmount);
            Assert.AreEqual(30, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(-10, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Budgeted_OneOperationChangeInCurrent_MultipleMonthsDuration_StartedInFutureMonth()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = new DateTime(testDate.Year, testDate.Month + 1, 1);
            moneyOperation.ValidityEndDate = testDate.AddMonths(5).AddDays(-testDate.Day+1).AddHours(-testDate.Hour+1).AddSeconds(-testDate.Second);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 4,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -10,
                ChangeDate = testDate,
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(30, operationSchedule.TotalAmount);
            Assert.IsNull(scheduleItem);
        }

        [Test]
        public void GetMoneyOperation_One_Budgeted_OneOperationChangeInFuture_MultipleMonthsDuration_AlreadyFinishedInPastMonth()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = new DateTime(testDate.Year, testDate.Month - 4, 1);
            moneyOperation.ValidityEndDate = FirstSecondOfCurrentMonth(testDate).AddSeconds(-1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 4,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -10,
                ChangeDate = testDate.AddMonths(1),
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(40, operationSchedule.TotalAmount);
            Assert.AreEqual(40, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(40, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(40, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(0, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Budgeted_OneOperationChangeInFuture_MultipleMonthsDuration_GettingFinishedInCurrentMonth()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = new DateTime(testDate.Year, testDate.Month - 3, 1);
            moneyOperation.ValidityEndDate = new DateTime(testDate.Year, testDate.Month + 1, 1).AddSeconds(-1);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 40;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 4,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -10,
                ChangeDate = testDate.AddMonths(1),
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(40, operationSchedule.InitialAmount);
            Assert.AreEqual(40, operationSchedule.TotalAmount);
            Assert.AreEqual(40, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(40, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(40, scheduleItem.TotalAmount);
            Assert.AreEqual(0, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(0, scheduleItem.CurrentChangeAmount);
        }

        [Test]
        public void GetMoneyOperation_One_Budgeted_OneOperationChangeInFuture_MultipleMonthsDuration_StartedInCurrentMonth()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = FirstSecondOfCurrentMonth(testDate);
            moneyOperation.ValidityEndDate = LastSecondOf2ndMonthFromCurrent(testDate);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 30;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 4,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -10,
                ChangeDate = testDate.AddMonths(1),
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(30, operationSchedule.InitialAmount);
            Assert.AreEqual(20, operationSchedule.TotalAmount);
            Assert.AreEqual(30, scheduleItem.TotalBudgetedAmount);
            Assert.AreEqual(10, scheduleItem.CurrentBudgetedAmount);
            Assert.AreEqual(30, scheduleItem.TotalAmount);
            Assert.AreEqual(20, scheduleItem.LeftBudgetedAmount);
            Assert.AreEqual(0, scheduleItem.CurrentChangeAmount);
        }

        private DateTime LastSecondOf2ndMonthFromCurrent(DateTime testDate)
        {
            var nextMonthDate = testDate.AddMonths(2);
            return new DateTime(nextMonthDate.Year, nextMonthDate.Month, DateTime.DaysInMonth(nextMonthDate.Year, nextMonthDate.Month), 23, 59, 59);
        }

        [Test]
        public void GetMoneyOperation_One_Budgeted_OneOperationChangeInFuture_MultipleMonthsDuration_StartedInFutureMonth()
        {
            var testDate = DateTime.UtcNow;
            //Setup
            var newContext = new FakeFinanceManagerContext();
            var moneyOperation = new MoneyOperation();
            moneyOperation.ID = 1;
            moneyOperation.ValidityBeginDate = FirstSecondOfNextMonth(testDate);
            moneyOperation.ValidityEndDate = LastSecondOf3rdMonthFromCurrent(testDate);
            moneyOperation.RepetitionUnit = PeriodUnit.Month;
            moneyOperation.RepetitionUnitQuantity = 1;
            moneyOperation.InitialAmount = 30;
            moneyOperation.IsActive = true;
            moneyOperation.IsReal = true;
            moneyOperation.OperationSetting = new MoneyOperationSetting()
            {
                ID = 1,
                ReservePeriodQuantity = 4,
                ReservePeriodUnit = PeriodUnit.Month
            };
            moneyOperation.OperationSettingID = moneyOperation.OperationSetting.ID;
            moneyOperation.MoneyOperationChanges.Add(new MoneyOperationChange
            {
                ID = 1,
                ChangeAmount = -10,
                ChangeDate = testDate.AddMonths(1),
                MoneyOperationID = moneyOperation.ID
            });

            newContext.MoneyOperations.Add(moneyOperation);

            SetContext(newContext);
            var operationSchedule = _moneyOperationsService.GetMoneyOperationSchedule(1, testDate);
            var scheduleItem = operationSchedule.ScheduleItem.FirstOrDefault(si => si.PeriodName == _moOpLogic.GetPeriodName(testDate));
            Assert.NotNull(operationSchedule);
            Assert.AreEqual(30, operationSchedule.InitialAmount);
            Assert.AreEqual(20, operationSchedule.TotalAmount);
            Assert.IsNull(scheduleItem);
        }

        private void SetContext(IFinanceManagerContext newContext)
        {
            _finManContext = newContext;
        }
    }
}
