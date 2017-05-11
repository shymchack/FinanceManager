using FinanceManager.DAL.Dtos;
using System.Collections.Generic;

namespace FinanceManager.DAL.Repositories
{
    public interface IAccountsRepository
    {
        List<AccountDto> GetAccounts();
        int CreateAccount(string name, int userID);
    }
}
