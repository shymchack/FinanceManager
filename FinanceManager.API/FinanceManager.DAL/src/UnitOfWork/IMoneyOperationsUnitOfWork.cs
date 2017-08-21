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
        void AddMoneyOperation(MoneyOperationDto moneyOperation);
    }
}
