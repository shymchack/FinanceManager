using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FinanceManager.DAL.UnitOfWork;
using FinanceManager.DAL.Dtos;
using FinanceManager.BL.ViewModels;
using FinanceManager.BL.UserInput;

namespace FinanceManager.API.Services
{
    public class MoneyOperationsService : IMoneyOperationsService
    {
        IMoneyOperationsUnitOfWork _moneyOperationUOW;
        IMoneyOperationLogic _moneyOperationLogic;

        public MoneyOperationsService(IMoneyOperationsUnitOfWork moneyOperationUOW, IMoneyOperationLogic moneyOperationLogic)
        {
            _moneyOperationUOW = moneyOperationUOW;
            _moneyOperationLogic = moneyOperationLogic;
        }

        public void AddMoneyOperation(MoneyOperationViewData moneyOperation)
        {
            MoneyOperationDto moneyOperationDto = _moneyOperationLogic.ConvertUserInputToDto(moneyOperation);
            _moneyOperationUOW.AddMoneyOperation(moneyOperationDto);
        }
    }
}