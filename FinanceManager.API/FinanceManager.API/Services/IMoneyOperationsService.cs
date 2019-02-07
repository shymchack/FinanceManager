using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FinanceManager.BL.UserInput;
using FinanceManager.API.Serialization;
using FinanceManager.BL;
using FinanceManager.API.Serialization.Types;

namespace FinanceManager.API.Services
{
    public interface IMoneyOperationsService
    {
        int AddMoneyOperation(MoneyOperationModel moneyOperation);
        MoneyOperationModel GetMoneyOperationById(int id);
        IEnumerable<MoneyOperationStatusModel> GetMoneyOperationsByAccountsIds(IEnumerable<int> enumerable, PeriodInfo periodInfo);
        MoneyOperationScheduleModel GetMoneyOperationSchedule(int moneyOperationId);
        void AddMoneyOperationChange(MoneyOperationChangeModel moneyOperationChange);
    }
}