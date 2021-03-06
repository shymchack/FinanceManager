﻿using FinanceManager.DAL.Dtos;
using System;
using System.Collections.Generic;

namespace FinanceManager.DAL.UnitOfWork
{
    public interface IMoneyOperationsUnitOfWork
    {
        int AddMoneyOperation(MoneyOperationDto moneyOperation);
        MoneyOperationDto GetMoneyOperationById(int moneyOperationId);
        IEnumerable<MoneyOperationDto> GetMoneyOperationsByAccountsIDs(IEnumerable<int> accountsIds, DateTime beginDate);
        int AddMoneyOperationChange(MoneyOperationChangeDto moneyOperationChangeDto);
    }
}
