using FinanceManager.DAL.Dtos;
using System;
using System.Collections.Generic;

namespace FinanceManager.DAL.UnitOfWork
{
    public interface IMoneyOperationsUnitOfWork
    {
        int AddMoneyOperation(MoneyOperationDto moneyOperation);
        MoneyOperationDto GetMoneyOperationById(int id);
        IEnumerable<MoneyOperationDto> GetMoneyOperationsByAccountsIDs(IEnumerable<int> accountsIds, DateTime beginDate, DateTime endDate);
        int AddMoneyOperationChange(MoneyOperationChangeDto moneyOperationChangeDto);
    }
}
