using FinanceManager.DAL.Dtos;
using FinanceManager.Database.Entities;

namespace FinanceManager.API.Services
{
    public interface IAccountsService
    {
        int CreateAccount(string name, int userID);
        AccountDto GetAccountByName(string v);
    }
}