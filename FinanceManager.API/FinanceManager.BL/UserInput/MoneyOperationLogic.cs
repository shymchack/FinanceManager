﻿using FinanceManager.DAL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinanceManager.Types.Enums;

namespace FinanceManager.BL.UserInput
{
    public class MoneyOperationLogic : IMoneyOperationLogic
    {
        public MoneyOperationDto ConvertUserInputToDto(MoneyOperationModel moneyOperation)
        {
            //TODO validate data conversion - most probably not all needed fields are included (or too much of them)
            if (IsValid(moneyOperation))
            {
                MoneyOperationDto moneyOperationDto = new MoneyOperationDto();
                moneyOperationDto.AccountID = moneyOperation.AccountID;
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

        public MoneyOperationModel ConvertDtoToViewData(MoneyOperationDto moneyOperationDto)
        {
            //TODO validate data conversion - most probably not all needed fields are included (or too much of them)
            MoneyOperationModel moneyOperationViewData = new MoneyOperationModel();
            moneyOperationViewData.Description = moneyOperationDto.Description;
            moneyOperationViewData.InitialAmount = moneyOperationDto.InitialAmount;
            moneyOperationViewData.IsActive = moneyOperationDto.IsActive;
            moneyOperationViewData.IsReal = moneyOperationDto.IsReal;
            moneyOperationViewData.Name = moneyOperationDto.Name;
            moneyOperationViewData.OperationSettingID = moneyOperationDto.OperationSettingID;
            moneyOperationViewData.RepetitionUnit = moneyOperationDto.RepetitionUnit;
            moneyOperationViewData.RepetitionUnitQuantity = moneyOperationDto.RepetitionUnitQuantity;
            moneyOperationViewData.ValidityBeginDate = moneyOperationDto.ValidityBeginDate;
            moneyOperationViewData.ValidityEndDate = moneyOperationDto.ValidityEndDate;

            return moneyOperationViewData;
        }

        public MoneyOperationStatus PrepareMoneyOperationStatus(MoneyOperationDto moneyOperationDto, DateTime date)
        {
            DateTime currentDate = date;
            short currentPaymentNumber = 0;
            short totalPaymentsNumber = 0;
            //TODO: Write some helper that would check only to repetitionUnit precision, for example reject minutes and seconds if unit is hour
            while(currentDate <= moneyOperationDto.ValidityEndDate)
            {
                if(currentDate < DateTime.UtcNow)
                {
                    currentPaymentNumber++;
                }
                totalPaymentsNumber++;
                currentDate = RepetitionUnitCalculator.CalculateNextRepetitionDate(currentDate, moneyOperationDto.RepetitionUnit, moneyOperationDto.RepetitionUnitQuantity);
            }

            //TimeSpan repetitionTimeSpan = RepetitionUnitCalculator.CalculateRepetitionTimeSpan(moneyOperationDto); // I don't know if this is necessary
            MoneyOperationStatus status = new MoneyOperationStatus();
            IEnumerable<MoneyOperationChangeDto> periodMoneyOperationChanges = ExtractCurrentPeriodOperations(moneyOperationDto);
            status.AccountID = moneyOperationDto.AccountID;
            status.InitialAmount = moneyOperationDto.InitialAmount;
            status.AlreadyPayedAmount = moneyOperationDto.MoneyOperationChanges.Sum(moc => -moc.ChangeAmount);
            status.CurrentPeriodPayedAmount = periodMoneyOperationChanges.Sum(moc => -moc.ChangeAmount);
            status.Description = moneyOperationDto.Description;
            status.Name = moneyOperationDto.Name;
            status.TotalBudgetedAmount = currentPaymentNumber / totalPaymentsNumber * status.InitialAmount - status.AlreadyPayedAmount; // TODO: Make sure it's needed to subtract already payed amount
            status.FinishDate = moneyOperationDto.ValidityEndDate;
            status.BeginningDate = moneyOperationDto.ValidityBeginDate;
            //TODO at first implement the "money operation freeze feature" - remember to rename FrozenAmount to prevent misunderstaindings.
            status.PeriodsLeftToPay = totalPaymentsNumber - currentPaymentNumber;
            //status.CurrPeriodIncomes = moneyOperationChanges.Where(mo => mo.ChangeAmount > 0).Sum(moc => moc.ChangeAmount);

            return status;
        }
        
        //TODO think abuot making this independent of UtcNow
        private IEnumerable<MoneyOperationChangeDto> ExtractCurrentPeriodOperations(MoneyOperationDto moneyOperationDto)
        {
            var repetitionTimeStamp = RepetitionUnitCalculator.CalculateRepetitionTimeStamp(DateTime.UtcNow, moneyOperationDto.RepetitionUnit, moneyOperationDto.RepetitionUnitQuantity);
            var currentPeriodChanges = moneyOperationDto.MoneyOperationChanges.Where(moc => ChallengeDateRepetitionProperties(moc.ChangeDate, moneyOperationDto.RepetitionUnit, moneyOperationDto.RepetitionUnitQuantity));
            return currentPeriodChanges;
        }

        private bool ChallengeDateRepetitionProperties(DateTime changeDate, PeriodUnit repetitionUnit, short repetitionUnitQuantity)
        {
            var currentPeriodBeginning = RepetitionUnitCalculator.ClearMinorDateTimePart(changeDate, repetitionUnit, repetitionUnitQuantity);
            var timestampToIncrement = RepetitionUnitCalculator.CalculateRepetitionTimeStamp(changeDate, repetitionUnit, repetitionUnitQuantity);
            var nextPeriodBegining = currentPeriodBeginning.Add(timestampToIncrement);

            return nextPeriodBegining >= DateTime.UtcNow && currentPeriodBeginning <= DateTime.UtcNow;
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

            DateTime nextExecutionDateCandidate = moneyOperationDto.LastOrFirstOperationExecutionDate;

            while(nextExecutionDateCandidate < DateTime.UtcNow)
            {
                DateTime originalCandidate = nextExecutionDateCandidate;
                nextExecutionDateCandidate = RepetitionUnitCalculator.CalculateNextRepetitionDate(nextExecutionDateCandidate, moneyOperationDto.RepetitionUnit, moneyOperationDto.RepetitionUnitQuantity);
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

        private bool IsValid(MoneyOperationModel moneyOperation)
        {
            return (moneyOperation.ValidityEndDate >= moneyOperation.ValidityBeginDate);
        }
    }
}
