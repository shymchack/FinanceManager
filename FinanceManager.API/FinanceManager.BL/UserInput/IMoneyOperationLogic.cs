using FinanceManager.BL.ViewModels;
using FinanceManager.DAL.Dtos;

namespace FinanceManager.BL.UserInput
{
    public interface IMoneyOperationLogic
    {
        MoneyOperationDto ConvertUserInputToDto(MoneyOperationViewData moneyOperation);
    }
}