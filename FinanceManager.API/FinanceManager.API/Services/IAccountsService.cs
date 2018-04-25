using System.Collections.Generic;
using FinanceManager.DAL.Dtos;

namespace FinanceManager.API.Services
{
    public interface IAccountsService
    {
        int CreateAccount(string name, int userID);
        AccountDto GetAccountByName(string v);
        IEnumerable<AccountDto> GetAccountsByUserId(int userId);
    }
}