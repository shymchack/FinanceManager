using System.Collections.Generic;
using System.Linq;
using FinanceManager.DAL.Dtos;
using System;
using FinanceManager.Database.Entities;
using FinanceManager.DAL.Repositories.Contracts;
using Financemanager.Database.Context;

namespace FinanceManager.DAL.Repositories
{
    public class AccountsRepository : FinanceManagerRepository, IDisposable
    {
        public AccountsRepository(FinanceManagerContext context): base(context)
        {
        }

        public List<AccountDto> GetAccounts()
        {
            //TODO: AUTOMAPPER!
            return Context.Accounts.Select(a => new AccountDto() { CurrentAmount = a.CurrentAmount, InitialAmount = a.InitialAmount }).ToList();
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
    }
}
