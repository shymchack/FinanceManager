using System;
using FinanceManager.BL.UserInput;
using FinanceManager.DAL.Dtos;

namespace FinanceManager.BL
{
    public interface IMoneyOperationLogic
    {
        MoneyOperationDto ConvertUserInputToDto(MoneyOperationViewData moneyOperation);
        MoneyOperationViewData ConvertDtoToViewData(MoneyOperationDto moneyOperationDto);
        MoneyOperationStatus PrepareMoneyOperationStatus(MoneyOperationDto moneyOperationDto, DateTime date);
    }
}