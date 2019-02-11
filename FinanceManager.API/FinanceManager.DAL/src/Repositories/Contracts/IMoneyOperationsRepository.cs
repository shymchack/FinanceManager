using FinanceManager.Database.Entities;
using System;
using System.Collections.Generic;

namespace FinanceManager.DAL.Repositories.Contracts
{
    public interface IMoneyOperationsRepository
    {
        MoneyOperation CreateMoneyOperation(Account account);
        void AddMoneyOperation(MoneyOperation moneyOperation);
        MoneyOperation GetMoneyOperationById(int id);
        IEnumerable<MoneyOperation> GetMoneyOperationsByAccountsIDs(IEnumerable<int> accountsIDs, DateTime endDate);
        MoneyOperationChange CreateMoneyOperationChange(MoneyOperation moneyOperation);
        void AddMoneyOperationChange(MoneyOperationChange moneyOperationChange);
    }
}
