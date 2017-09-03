using FinanceManager.BD.UserInput;
using FinanceManager.DAL.Dtos;

namespace FinanceManager.BD.UserInput
{
    public interface IMoneyOperationLogic
    {
        MoneyOperationDto ConvertUserInputToDto(MoneyOperationViewData moneyOperation);
        MoneyOperationViewData ConvertDtoToViewData(MoneyOperationDto moneyOperationDto);
    }
}