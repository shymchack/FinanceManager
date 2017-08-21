using FinanceManager.BL.ViewModels;
using FinanceManager.DAL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.BL.UserInput
{
    public class MoneyOperationLogic : IMoneyOperationLogic
    {
        public MoneyOperationDto ConvertUserInputToDto(MoneyOperationViewData moneyOperation)
        {
            if (IsValid(moneyOperation))
            {
                MoneyOperationDto moneyOperationDto = new MoneyOperationDto();
                moneyOperationDto.AccountID = GetCurrentAccountID();
                moneyOperationDto.Description = moneyOperation.Description;
                moneyOperationDto.InitialAmount = moneyOperation.InitialAmount;
                moneyOperationDto.IsActive = moneyOperation.IsActive;
                moneyOperationDto.IsAlreadyProcessed = false;
                moneyOperationDto.IsReal = moneyOperation.IsReal;
                moneyOperationDto.Name = moneyOperation.Name;
                moneyOperationDto.OperationSettingID = moneyOperation.OperationSettingID;
                moneyOperationDto.RepetitionUnit = moneyOperation.RepetitionUnit;
                moneyOperationDto.RepetitionUnitQuantity = moneyOperation.RepetitionUnitQuantity;
                moneyOperationDto.ValidityBeginDate = moneyOperation.ValidityBeginDate;
                moneyOperationDto.ValidityEndDate = moneyOperation.ValidityEndDate;
                moneyOperationDto.NextOperationExecutionDate = CalculateNextOperationExecutionDate(moneyOperationDto);
                return moneyOperationDto;
            }
            return null;
        }

        private DateTime CalculateNextOperationExecutionDate(MoneyOperationDto moneyOperationDto)
        {
            if (IsSingleOperation(moneyOperationDto))
                return CalculateNextOperationExecutionDateForSingleOperation(moneyOperationDto);
            else
                return CalculateNextOperationExecutionDateForRecurrentOperation(moneyOperationDto);
        }

        private DateTime CalculateNextOperationExecutionDateForSingleOperation(MoneyOperationDto moneyOperationDto)
        {
            return moneyOperationDto.ValidityBeginDate;
        }

        /// <summary>
        /// Returns first operation execution date that comes from period between now and the validity end date if exists. 
        /// Otherwise returns last operation execution date.
        /// </summary>
        /// <param name="moneyOperationDto"></param>
        /// <returns></returns>
        private DateTime CalculateNextOperationExecutionDateForRecurrentOperation(MoneyOperationDto moneyOperationDto)
        {
            if (moneyOperationDto.ValidityBeginDate >= moneyOperationDto.ValidityEndDate)
            {
                throw new Exception("This is not a recurrent operation");
            }

            RepetitionUnitCalculator repetitionUnitCalculator = new RepetitionUnitCalculator();
            DateTime nextExecutionDateCandidate = moneyOperationDto.LastOrFirstOperationExecutionDate;

            while(nextExecutionDateCandidate < DateTime.UtcNow)
            {
                DateTime originalCandidate = nextExecutionDateCandidate;
                nextExecutionDateCandidate = repetitionUnitCalculator.CalculateNextRepetitionDate(nextExecutionDateCandidate, moneyOperationDto.RepetitionUnit, moneyOperationDto.RepetitionUnitQuantity);
                if (originalCandidate >= nextExecutionDateCandidate)
                {
                    throw new Exception("Next operation execution date candidate has been overtaken by previous one.");
                }
            }

            return nextExecutionDateCandidate;
        }

        private bool IsSingleOperation(MoneyOperationDto moneyOperationDto)
        {
            return moneyOperationDto.ValidityBeginDate == moneyOperationDto.ValidityEndDate;
        }

        private bool IsValid(MoneyOperationViewData moneyOperation)
        {
            return (moneyOperation.ValidityEndDate >= moneyOperation.ValidityBeginDate);
        }

        private int GetCurrentAccountID()
        {
            throw new NotImplementedException();
        }
    }
}
