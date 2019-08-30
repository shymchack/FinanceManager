using FinanceManager.API.Services;
using NUnit.Framework;
using FinanceManager.DAL.Repositories.Contracts;
using FinanceManager.DAL;
using FinanceManager.Database.Context;
using FinanceManager.DAL.Repositories;
using FinanceManager.DAL.Dtos;

namespace FinanceManager.API.Tests.Services
{
    [TestFixture]
    public class AccountsServiceTest
    {
        public IAccountsService AccountsService;

        public AccountsServiceTest()
        {
            IFinanceManagerContext context = new FakeFinanceManagerContext();
            IAccountsRepository accRepo = new AccountsRepository(context);
            IUsersRepository usersRepo = new UsersRepository(context);
            UserAccountsUnitOfWork userAccountsUOW = new UserAccountsUnitOfWork(context, usersRepo, accRepo);
            AccountsService = new AccountsService(userAccountsUOW);
        }

        [Test]
        public void CreateAccountTest()
        {
            AccountsService.CreateAccount("name", 2);
            AccountDto account = AccountsService.GetAccountByName("name");
            Assert.AreEqual("name", account.Name);
        }
    }
}
