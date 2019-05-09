using System;
using NUnit.Framework;
using FinanceManager.API.Services;
using FinanceManager.DAL.Repositories;
using FinanceManager.DAL.Repositories.Contracts;
using FinanceManager.Database.Context;
using FinanceManager.DAL.UnitOfWork;
using FinanceManager.BL.UserInput;
using FinanceManager.Types.Enums;
using FinanceManager.DAL;
using Moq;
using FinanceManager.DAL.Dtos;
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

        private void SetContext(IFinanceManagerContext newContext)
        {
            _finManContext = newContext;
        }
    }
}
