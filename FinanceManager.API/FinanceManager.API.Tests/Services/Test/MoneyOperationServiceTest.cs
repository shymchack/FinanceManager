﻿using System;
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


        private void SetContext(IFinanceManagerContext newContext)
        {
            _finManContext = newContext;
        }
    }
}
