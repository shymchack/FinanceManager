using FinanceManager.DAL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.DAL.UnitOfWork
{
    public interface IMoneyOperationsUnitOfWork
    {
        int AddMoneyOperation(MoneyOperationDto moneyOperation);
        MoneyOperationDto GetMoneyOperationById(int id);
    }
}
