using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FinanceManager.BD.UserInput;

namespace FinanceManager.API.Services
{
    public interface IMoneyOperationsService
    {
        int AddMoneyOperation(MoneyOperationViewData moneyOperation);
        MoneyOperationViewData GetMoneyOperationById(int id);
    }
}