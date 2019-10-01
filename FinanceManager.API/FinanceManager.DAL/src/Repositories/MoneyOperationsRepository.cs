using FinanceManager.DAL.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using FinanceManager.Database.Entities;
using FinanceManager.Database.Context;
using System.Data.Entity;
using Z.EntityFramework.Plus;

namespace FinanceManager.DAL.Repositories
{
    public class MoneyOperationsRepository : FinanceManagerRepository, IMoneyOperationsRepository
    {
        public MoneyOperationsRepository(IFinanceManagerContext context) : base(context)
        {
        }

        public MoneyOperation CreateMoneyOperation(Account account)
        {
            MoneyOperation moneyOperation = Context.MoneyOperations.Create();
            moneyOperation.Account = account;
            Context.MoneyOperations.Add(moneyOperation);
            return moneyOperation;
        }

        public void AddMoneyOperation(MoneyOperation moneyOperation)
        {
            if (Context.Entry(moneyOperation).State == EntityState.Added)
            {
                Context.SaveChanges();
            }
        }

        public MoneyOperation GetMoneyOperationById(int id)
        {
            return Context.MoneyOperations.FirstOrDefault(mo => mo.ID == id);
        }

        /// <summary>
        /// Get all money operations that have beginning date before the given endDate. Validity end date is not used here because we also need to 
        /// get all money operations that have not been cleared yet (incomes != expenses).
        /// </summary>
        /// <param name="accountsIDs"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public IEnumerable<MoneyOperation> GetMoneyOperationsByAccountsIDs(IEnumerable<int> accountsIDs, DateTime endDate)
        {
            //TODO optimize - prevent getting old operations (end of validity less than beginDate) that are clear (incomes == expenses)
            return Context.MoneyOperations
                .Where(mo => accountsIDs.Contains(mo.AccountID.Value) && mo.ValidityBeginDate <= endDate)
                .IncludeOptimized(mo => mo.Account)
                .IncludeOptimized(mo => mo.MoneyOperationChanges)
                .IncludeOptimized(mo => mo.OperationSetting);
        }

        public MoneyOperationChange CreateMoneyOperationChange(MoneyOperation moneyOperation)
        {
            MoneyOperationChange moneyOperationChange = Context.MoneyOperationChanges.Create();
            moneyOperationChange.MoneyOperation = moneyOperation;
            Context.MoneyOperationChanges.Add(moneyOperationChange);
            return moneyOperationChange;
        }

        public void AddMoneyOperationChange(MoneyOperationChange moneyOperationChange)
        {
            if (Context.Entry(moneyOperationChange).State == EntityState.Added)
            {
                Context.SaveChanges();
            }
        }
    }
}
