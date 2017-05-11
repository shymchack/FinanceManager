using Financemanager.Database.Context;
using System.Collections.Generic;
using System.Linq;
using FinanceManager.DAL.Dtos;
using System;
using FinanceManager.Database.Entities;

namespace FinanceManager.DAL.Repositories
{
    public class AccountsRepository : FinanceManagerRepository, IAccountsRepository, IDisposable
    {
        public AccountsRepository() : base()
        {

        }

        public List<AccountDto> GetAccounts()
        {
            //TODO: AUTOMAPPER!
            return Context.Accounts.Select(a => new AccountDto() { CurrentAmount = a.CurrentAmount, InitialAmount = a.InitialAmount }).ToList();
        }

        public int CreateAccount(string name, int userID)
        {
            Account newAccount = Context.Accounts.Create();
            newAccount.CreationDate = DateTime.UtcNow;
            newAccount.Name = name;

            User user = Context.Users.FirstOrDefault(u => u.ID == userID);
            if (user != null)
            {
                UserAccount userAccount = new UserAccount();
                userAccount.User = user;
                newAccount.UsersAccounts.Add(userAccount);
            }
            Context.Accounts.Add(newAccount);
            Context.SaveChanges();
            return newAccount.ID;
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
