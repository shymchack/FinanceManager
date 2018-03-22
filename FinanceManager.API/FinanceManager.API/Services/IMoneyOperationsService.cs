using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FinanceManager.BD.UserInput;
using FinanceManager.API.Serialization;
using FinanceManager.BD;

namespace FinanceManager.API.Services
{
    public interface IMoneyOperationsService
    {
        int AddMoneyOperation(MoneyOperationViewData moneyOperation);
        MoneyOperationViewData GetMoneyOperationById(int id);
        IEnumerable<MoneyOperationStatus> GetMoneyOperationsByAccountID(int accountId, DateTime date);
    }
}