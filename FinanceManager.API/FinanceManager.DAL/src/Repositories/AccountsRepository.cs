using System.Collections.Generic;
using System.Linq;
using FinanceManager.DAL.Dtos;
using System;
using FinanceManager.Database.Entities;
using FinanceManager.DAL.Repositories.Contracts;
using Financemanager.Database.Context;

namespace FinanceManager.DAL.Repositories
{
    public class AccountsRepository : FinanceManagerRepository, IAccountsRepository, IDisposable
    {
        public AccountsRepository(IFinanceManagerContext context): base(context)
        { }

        public List<AccountDto> GetAccounts()
        {
            //TODO: AUTOMAPPER!
            return Context.Accounts.Select(a => new AccountDto() { CurrentAmount = a.CurrentAmount, InitialAmount = a.InitialAmount, Name = a.Name}).ToList();
        }

        public int AddAccount(Account account)
        {
            Context.Accounts.Add(account);
            Context.SaveChanges();
            return account.ID;
        }

        public void Dispose()
        {
            Context.Dispose();
        }

        public Account CreateAccount()
        {
            Account account = Context.Accounts.Create();
            return account;
        }

        public Account GetAccountByID(int accountID)
        {
            return Context.Accounts.FirstOrDefault(a => a.ID == accountID);
        }
    }
}
