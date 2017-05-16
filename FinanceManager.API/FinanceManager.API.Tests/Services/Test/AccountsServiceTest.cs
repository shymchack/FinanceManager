using FinanceManager.API.Services;
using Moq;
using NUnit.Framework;
using FinanceManager.DAL.Repositories.Contracts;

namespace FinanceManager.API.Tests.Services
{
    [TestFixture]
    public class AccountsServiceTest
    {
        public IAccountsService AccountsService;
        public Mock<IAccountsRepository> AccountsRepositoryMock { get; set; }

        public AccountsServiceTest()
        {
            //AccountsRepositoryMock = new Mock<IAccountsRepository>();
            //AccountsRepositoryMock.Setup(s => s.CreateAccount("name", 1)).Returns(1);
            //AccountsService = new AccountsService();
        }

        [Test]
        public void CreateAccountTest()
        {
            //int newId = AccountsService.CreateAccount("name", 1);
            //Assert.AreEqual(newId, 1);
        }
    }
}
