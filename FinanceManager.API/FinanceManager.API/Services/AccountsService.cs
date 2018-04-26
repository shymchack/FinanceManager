using System;
using FinanceManager.DAL;
using System.Linq;
using FinanceManager.DAL.Dtos;
using System.Collections.Generic;
using FinanceManager.DAL.UnitOfWork;

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

        public IEnumerable<AccountDto> GetAccountsByUserId(int userId)
        {
            return UserAccountsUnitOfWork.GetAccountsByUserId(userId);
        }
    }
}