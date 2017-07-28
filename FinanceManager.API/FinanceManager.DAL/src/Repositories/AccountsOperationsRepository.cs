using FinanceManager.DAL.src.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinanceManager.Database.Entities;
using FinanceManager.DAL.Repositories;
using Financemanager.Database.Context;

namespace FinanceManager.DAL.src.Repositories
{
    public class AccountsOperationsRepository : FinanceManagerRepository, IAccountsOperationsRepository
    {
        public AccountsOperationsRepository(IFinanceManagerContext context) : base(context)
        {
        }

        public void CreateMoneyOperation(MoneyOperation moneyOperation)
        {
            if (moneyOperation.InitialAmount == 0)
            {
                throw new Exception("Initial amount could not be less than 0.");
            }
            Context.MoneyOperations.Add(moneyOperation);
        }

        public void GetAllOperationsByAccountId(int accountId)
        {
            var periodicExpenses = Context.MoneyOperations.Where(pe => pe.InitialAmount < 0 && pe.Account != null && pe.Account.ID == accountId && pe.IsActive);
            var periodicIncomes = Context.MoneyOperations.Where(pi => pi.InitialAmount > 0 && pi.Account != null && pi.Account.ID == accountId && pi.IsActive);
            var singleExpenses = Context.MoneyOperations.Where(se => se.InitialAmount < 0 && se.Account != null && se.Account.ID == accountId && se.IsActive);
            var singleIncomes = Context.MoneyOperations.Where(si => si.InitialAmount > 0 && si.Account != null && si.Account.ID == accountId && si.IsActive);
        }
    }
}
