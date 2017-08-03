using FinanceManager.API.Serialization.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinanceManager.API.Services
{
    public interface IMoneyOperationsService
    {
        void AddMoneyOperation(MoneyOperationViewData moneyOperation);
    }
}