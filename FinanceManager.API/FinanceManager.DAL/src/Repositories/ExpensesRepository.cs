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
    public class ExpensesRepository : FinanceManagerRepository, IExpensesRepository
    {
        public ExpensesRepository(IFinanceManagerContext context) : base(context)
        {
        }

        public void CreateSingleExpense(SingleExpense singleExpense)
        {
            if (Validate(singleExpense))
                Context.SingleExpenses.Add(singleExpense);
        }

        private bool Validate(SingleExpense singleExpense)
        {
            throw new NotImplementedException();
        }
    }
}
