using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FinanceManager.BL.ViewModels;

namespace FinanceManager.API.Services
{
    public interface IMoneyOperationsService
    {
        void AddMoneyOperation(MoneyOperationViewData moneyOperation);
    }
}