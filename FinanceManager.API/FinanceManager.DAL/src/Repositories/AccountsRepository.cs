using Financemanager.Database.Context;
using System.Collections.Generic;
using System.Linq;
using FinanceManager.DAL.Dtos;

namespace FinanceManager.DAL.Repositories
{
    public class AccountsRepository : IAccountsRepository
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

        public AccountsRepository(FinanceManagerContext context)
        {
            Context = context;
        }

        public List<AccountDto> GetAccounts()
        {
            //TODO: AUTOMAPPER!
            return Context.Accounts.Select(a => new AccountDto() { CurrentAmount = a.CurrentAmount, InitialAmount = a.InitialAmount }).ToList();
        }
    }
}
