using System;
using FinanceManager.BL.UserInput;
using FinanceManager.DAL.Dtos;

namespace FinanceManager.BL
{
    public interface IMoneyOperationLogic
    {
        MoneyOperationDto ConvertUserInputToDto(MoneyOperationModel moneyOperation);
        MoneyOperationModel ConvertDtoToViewData(MoneyOperationDto moneyOperationDto);
        MoneyOperationStatus PrepareMoneyOperationStatus(MoneyOperationDto moneyOperationDto, DateTime date);
    }
}