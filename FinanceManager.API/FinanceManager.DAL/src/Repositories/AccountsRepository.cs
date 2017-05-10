using Financemanager.Database.Context;
using System.Collections.Generic;
using System.Linq;
using FinanceManager.DAL.Dtos;
using System;
using FinanceManager.Database.Entities;

namespace FinanceManager.DAL.Repositories
{
    public class AccountsRepository : IAccountsRepository, IDisposable
    {
        private FinanceManagerContext _context;
        public FinanceManagerContext Context
        {
            get
            {
                return _context;
            }

            set
            {
                if (value != _context)
                    _context = value;
            }
        }

        public AccountsRepository()
        {

        }

        public AccountsRepository(FinanceManagerContext context)
        {
            Context = context;
        }

        public List<AccountDto> GetAccounts()
        {
            //TODO: AUTOMAPPER!
            return Context.Accounts.Select(a => new AccountDto() { CurrentAmount = a.CurrentAmount, InitialAmount = a.InitialAmount }).ToList();
        }

        public int CreateAccount(string name)
        {
            Account newAccount = Context.Accounts.Create();
            newAccount.CreationDate = DateTime.UtcNow;
            newAccount.Name = name;
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
