using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FinanceManager.DAL.UnitOfWork;
using FinanceManager.DAL.Dtos;
using FinanceManager.BD.UserInput;

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

        public int AddMoneyOperation(MoneyOperationViewData moneyOperation)
        {
            MoneyOperationDto moneyOperationDto = _moneyOperationLogic.ConvertUserInputToDto(moneyOperation);
            int newMoneyOperationId = _moneyOperationUOW.AddMoneyOperation(moneyOperationDto);

            //TODO pass response that would be more verbose than just an ID
            return newMoneyOperationId;
        }

        public MoneyOperationViewData GetMoneyOperationById(int id)
        {
            MoneyOperationDto moneyOperationDto = _moneyOperationUOW.GetMoneyOperationById(id);
            MoneyOperationViewData viewData = _moneyOperationLogic.ConvertDtoToViewData(moneyOperationDto);
            return viewData;
        }
    }
}