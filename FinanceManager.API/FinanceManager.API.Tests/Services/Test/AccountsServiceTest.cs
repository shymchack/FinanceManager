using FinanceManager.API.Services;
using Moq;
using FinanceManager.DAL.Repositories;
using NUnit;
using NUnit.Framework;

namespace FinanceManager.API.Tests.Services
{
    [TestFixture]
    public class AccountsServiceTest
    {
        public IAccountsService AccountsService;
        public Mock<IAccountsRepository> AccountsRepositoryMock { get; set; }

        public AccountsServiceTest()
        {
            AccountsRepositoryMock = new Mock<IAccountsRepository>();
            AccountsRepositoryMock.Setup(s => s.CreateAccount("name")).Returns(1);
            AccountsService = new AccountsService(AccountsRepositoryMock.Object);
        }

        [Test]
        public void CreateAccountTest()
        {
            int newId = AccountsService.CreateAccount("name");
            Assert.AreEqual(newId, 1);
        }
    }
}
