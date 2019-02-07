using System;
using FinanceManager.BL.UserInput;
using FinanceManager.DAL.Dtos;

namespace FinanceManager.BL
{
    public interface IMoneyOperationLogic
    {
        MoneyOperationDto ConvertUserInputToDto(MoneyOperationModel moneyOperation);
        MoneyOperationModel ConvertDtoToViewData(MoneyOperationDto moneyOperationDto);
        MoneyOperationStatusModel PrepareMoneyOperationStatus(MoneyOperationDto moneyOperationDto, PeriodInfo periodInfo);
        MoneyOperationScheduleModel GetMoneyOperationSchedule(MoneyOperationDto moneyOperationDto);
        MoneyOperationChangeDto ConvertMoneyOperationChangeUserInputToDto(MoneyOperationChangeModel moneyOperationChange);
        bool IsOperationBudgeted(MoneyOperationDto mo);
        bool IsOperationCyclic(MoneyOperationDto mo);
        bool IsOperationSingle(MoneyOperationDto mo);
    }
}