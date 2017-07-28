using FinanceManager.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.DAL.src.Repositories.Contracts
{
    public interface IAccountsOperationsRepository
    {
        void CreateSingleExpense(SingleExpense singleExpense);
}
