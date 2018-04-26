using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FinanceManager.BL.UserInput;
using FinanceManager.API.Serialization;
using FinanceManager.BL;

namespace FinanceManager.API.Services
{
    public interface IMoneyOperationsService
    {
        int AddMoneyOperation(MoneyOperationViewData moneyOperation);
        MoneyOperationViewData GetMoneyOperationById(int id);
        IEnumerable<MoneyOperationStatus> GetMoneyOperationsByAccountsIds(IEnumerable<int> accountId, DateTime date);
    }
}