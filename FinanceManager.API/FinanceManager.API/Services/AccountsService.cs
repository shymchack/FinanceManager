using FinanceManager.DAL.Repositories.Contracts;
using FinanceManager.DAL.UnitOfWork;

namespace FinanceManager.API.Services
{
    public class AccountsService : IAccountsService
    {
        public UserAccountsUnitOfWork UserAccountsUnitOfWork { get; set; }


        public AccountsService()
        {
            UserAccountsUnitOfWork = new UserAccountsUnitOfWork();
        }


        public int CreateAccount(string name, int userID)
        {
            return UserAccountsUnitOfWork.CreateAccount(name, userID);
        }
    }
}