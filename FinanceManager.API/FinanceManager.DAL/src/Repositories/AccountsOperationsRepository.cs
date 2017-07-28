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

        public void CreateSingleExpense(SingleExpense singleExpense)
        {
            Context.SingleExpenses.Add(singleExpense);
        }

        public void GetAllOperationsByAccountId(int accountId)
        {
            var periodicExpenses = Context.PeriodicExpenses.Where(pe => pe.Account != null && pe.Account.ID == accountId && pe.IsActive);
            var periodicIncomes = Context.PeriodicIncomes.Where(pi => pi.Account != null && pi.Account.ID == accountId && pi.IsActive);
            var singleExpenses = Context.SingleExpenses.Where(se => se.Account != null && se.Account.ID == accountId && se.IsActive);
            var singleIncomes = Context.SingleIncomes.Where(si => si.Account != null && si.Account.ID == accountId && si.IsActive);
        }
    }
}
