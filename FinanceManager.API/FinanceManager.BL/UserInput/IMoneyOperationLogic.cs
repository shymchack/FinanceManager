using System;
using FinanceManager.BD.UserInput;
using FinanceManager.DAL.Dtos;

namespace FinanceManager.BD
{
    public interface IMoneyOperationLogic
    {
        MoneyOperationDto ConvertUserInputToDto(MoneyOperationViewData moneyOperation);
        MoneyOperationViewData ConvertDtoToViewData(MoneyOperationDto moneyOperationDto);
        MoneyOperationStatus PrepareMoneyOperationStatus(MoneyOperationDto moneyOperationDto, DateTime date);
    }
}