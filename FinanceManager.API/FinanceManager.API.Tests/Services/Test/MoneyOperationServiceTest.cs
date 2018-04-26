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

namespace FinanceManager.API.Tests.Services
{
    [TestFixture]
    public class MoneyOperationsServiceTest
    {

        public IMoneyOperationsService MoneyOperationsService;
        public IMoneyOperationLogic MoneyOperationLogic;

        public MoneyOperationsServiceTest()
        {
        }

        [Test]
        public void AddMoneyOperationTest()
        {
            int fakeID = new Random().Next();

            MoneyOperationModel data = new MoneyOperationModel();
            data.AccountID = new Random().Next();
            data.Description = "Some test description";
            data.InitialAmount = new Random().Next();
            data.IsActive = true;
            data.IsReal = true;
            data.Name = "A test money operation";
            data.RepetitionUnit = PeriodUnit.Month;
            data.RepetitionUnitQuantity = 1;
            data.ValidityBeginDate = DateTime.UtcNow.AddDays(-10);
            data.ValidityEndDate = DateTime.UtcNow.AddMonths(2);

            MoneyOperationDto fakeDto = new MoneyOperationDto();
            Mock<IMoneyOperationLogic> moOpLogMock = new Mock<IMoneyOperationLogic>();
            moOpLogMock.Setup(m => m.ConvertUserInputToDto(data)).Returns(fakeDto);
            Mock<IMoneyOperationsUnitOfWork> moneyOperationUOWMock = new Mock<IMoneyOperationsUnitOfWork>();
            MoneyOperationsService = new MoneyOperationsService(moneyOperationUOWMock.Object, moOpLogMock.Object);
            moneyOperationUOWMock.Setup(m => m.AddMoneyOperation(fakeDto)).Returns(fakeID);

            int id = MoneyOperationsService.AddMoneyOperation(data);

            Assert.AreEqual(id, fakeID);
        }

    }
}
