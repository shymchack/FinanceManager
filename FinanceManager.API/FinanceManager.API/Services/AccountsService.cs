using FinanceManager.DAL;

namespace FinanceManager.API.Services
{
    public class AccountsService : IAccountsService
    {
        public IUserAccountsUnitOfWork UserAccountsUnitOfWork { get; set; }


        public AccountsService(IUserAccountsUnitOfWork unitOfWork)
        {
            UserAccountsUnitOfWork = unitOfWork;
        }


        public int CreateAccount(string name, int userID)
        {
            return UserAccountsUnitOfWork.CreateAccount(name, userID);
        }
    }
}