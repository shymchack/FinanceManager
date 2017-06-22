using System;
using FinanceManager.DAL;
using FinanceManager.Database.Entities;
using System.Linq;
using FinanceManager.DAL.Dtos;

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

        public AccountDto GetAccountByName(string name)
        {
            return UserAccountsUnitOfWork.GetAccounts().FirstOrDefault(a => a.Name == name);
        }
    }
}