using FinanceManager.API.Services;
using Moq;
using NUnit.Framework;
using FinanceManager.DAL.Repositories.Contracts;
using FinanceManager.DAL;

namespace FinanceManager.API.Tests.Services
{
    [TestFixture]
    public class AccountsServiceTest
    {
        public IAccountsService AccountsService;
        public Mock<IAccountsRepository> AccountsRepositoryMock { get; set; }

        public AccountsServiceTest()
        {
            var mock = new Mock<IUserAccountsUnitOfWork>();
            mock.Setup(s => s.CreateAccount("name", 2)).Returns(1);
            AccountsService = new AccountsService(mock.Object);
        }

        [Test]
        public void CreateAccountTest()
        {
            int newId = AccountsService.CreateAccount("name", 2);
            Assert.AreEqual(newId, 1);
        }
    }
}
