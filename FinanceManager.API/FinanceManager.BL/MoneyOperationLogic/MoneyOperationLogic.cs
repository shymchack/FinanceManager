using FinanceManager.DAL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using FinanceManager.Types.Enums;
using System.Globalization;
using FinanceManager.BL.Metadata;
using FinanceManager.BL.UserInput;

namespace FinanceManager.BL
{
    public class MoneyOperationLogic : IMoneyOperationLogic
    {
        public MoneyOperationDto ConvertUserInputToDto(MoneyOperationModel moneyOperation)
        {
            return MapUserInputToDto(moneyOperation);
        }

        public MoneyOperationModel ConvertDtoToViewData(MoneyOperationDto moneyOperationDto)
        {
            return MapDtoToViewData(moneyOperationDto);
        }

        public MoneyOperationStatusModel PrepareMoneyOperationStatus(MoneyOperationDto moneyOperationDto, DateTime targetDate)
        {
            if (targetDate < moneyOperationDto.ValidityBeginDate || targetDate > moneyOperationDto.ValidityEndDate) return null; //TODO: maybe should return new()?Metad

            var initialAmount = moneyOperationDto.InitialAmount;

            MoneyOperationStatusModel status = new MoneyOperationStatusModel();
            status.AccountID = moneyOperationDto.AccountID;
            status.Description = moneyOperationDto.Description;
            status.Name = moneyOperationDto.Name;
            status.FinishDate = moneyOperationDto.ValidityEndDate;
            status.BeginningDate = moneyOperationDto.ValidityBeginDate;
            status.InitialAmount = initialAmount;

            MoneyOperationPeriodMetadata periodMetadata = ReadCurrentPeriodMetadata(moneyOperationDto, targetDate);
            IEnumerable<MoneyOperationChangeDto> periodMoneyOperationChanges = ExtractPeriodMoneyOperationChanges(moneyOperationDto, targetDate);
            var currentPeriodPayedAmount = periodMoneyOperationChanges.Sum(moc => -moc.ChangeAmount);

            decimal periodBeginningAmount = initialAmount;
            decimal alreadyPayedAmountBeforeCurrent = 0;
            decimal currentAmount = initialAmount;
            decimal currentPeriodEndAmount = currentAmount;
            decimal currentPeriodBudgetedAmount = 0;

            if (periodMetadata == null)
            {
                periodBeginningAmount -= alreadyPayedAmountBeforeCurrent;
            }
            else
            {
                var periodsLeftToPayIncludingNow = periodMetadata.TotalPaymentsNumber - periodMetadata.NowPaymentNumber + 1;
                alreadyPayedAmountBeforeCurrent = moneyOperationDto.MoneyOperationChanges.Where(moc => moc.ChangeDate < periodMetadata.CurrentPeriodBeginningDate).Sum(moc => -moc.ChangeAmount);
                //TODO should I round the date up to end of current period? Think about periodMetadata.NowPaymentNumber usage below
                currentAmount -= (alreadyPayedAmountBeforeCurrent + currentPeriodPayedAmount);
                if (targetDate >= periodMetadata.NowPeriodBeginningDate)
                {
                    currentPeriodBudgetedAmount = periodsLeftToPayIncludingNow > 0 ? Math.Max(0, (periodBeginningAmount / periodsLeftToPayIncludingNow)) : 0;
                    if (targetDate < periodMetadata.NowPeriodEndDate)
                    {
                        currentPeriodBudgetedAmount -= currentPeriodPayedAmount;
                    }
                }

                currentPeriodEndAmount -= currentPeriodBudgetedAmount;
            }
            
            status.CurrentAmount = currentAmount;
            status.AlreadyPayedAmount = alreadyPayedAmountBeforeCurrent;
            status.CurrentPeriodPayedAmount = currentPeriodPayedAmount;
            status.CurrentPeriodBudgetedAmount = currentPeriodBudgetedAmount;
            status.CurrentPeriodEndAmount = currentPeriodEndAmount;

            return status;
        }

        public MoneyOperationScheduleModel GetMoneyOperationSchedule(MoneyOperationDto moneyOperationDto)
        {
            MoneyOperationScheduleModel moneyOperationScheduleModel = new MoneyOperationScheduleModel();

            moneyOperationScheduleModel.Name = moneyOperationDto.Name;
            moneyOperationScheduleModel.TotalAmount = (double) (moneyOperationDto.InitialAmount + moneyOperationDto.MoneyOperationChanges.Sum(moc => -moc.ChangeAmount));
            moneyOperationScheduleModel.InitialAmount = (double) moneyOperationDto.InitialAmount;
            DateTime date = moneyOperationDto.ValidityBeginDate;
            var scheduleItems = new List<MoneyOperationScheduleItemModel>();
            var totalBudgetedAmount = 0d;
            while (date <= moneyOperationDto.ValidityEndDate)
            {
                var moneyOperationStatus = PrepareMoneyOperationStatus(moneyOperationDto, date);
                var itemBalance = (double)moneyOperationStatus.CurrentAmount;
                var budgetedAmount = Math.Max(0,(double)moneyOperationStatus.CurrentPeriodBudgetedAmount);
                var payedAmount = (double) moneyOperationStatus.CurrentPeriodPayedAmount;
                totalBudgetedAmount += budgetedAmount;
                MoneyOperationScheduleItemModel scheduleItem = new MoneyOperationScheduleItemModel();
                scheduleItem.ItemBalance = itemBalance;
                scheduleItem.BudgetedAmount = budgetedAmount;
                scheduleItem.TotalBudgetedAmount = Math.Max(0, totalBudgetedAmount);
                scheduleItem.PayedAmount = (double)moneyOperationStatus.CurrentPeriodPayedAmount;
                scheduleItem.ItemBudgetedBalance = Math.Max(0, itemBalance - Math.Max(0, totalBudgetedAmount));
                scheduleItem.PeriodName = date.ToString("MMMM", CultureInfo.InvariantCulture); // TODO this is so bad to use month name. Remember that not only months can be processed.
                date = RepetitionUnitCalculator.CalculateNextRepetitionDate(date, moneyOperationDto.RepetitionUnit, moneyOperationDto.RepetitionUnitQuantity);

                scheduleItems.Add(scheduleItem);
            }
            moneyOperationScheduleModel.ScheduleItem = scheduleItems;
            return moneyOperationScheduleModel;
        }
        public MoneyOperationChangeDto ConvertMoneyOperationChangeUserInputToDto(MoneyOperationChangeModel moneyOperationChange)
        {
            return MapMoneyOperationChangeUserInputToDto(moneyOperationChange);
        }

        private MoneyOperationChangeDto MapMoneyOperationChangeUserInputToDto(MoneyOperationChangeModel moneyOperationChange)
        {
            var moneyOperationChangeDto = new MoneyOperationChangeDto();
            moneyOperationChangeDto.ChangeAmount = moneyOperationChange.ChangeAmount;
            moneyOperationChangeDto.ChangeDate = moneyOperationChange.ChangeDate;
            moneyOperationChangeDto.MoneyOperationID = moneyOperationChange.MoneyOperationId;

            return moneyOperationChangeDto;
        }

        private MoneyOperationDto MapUserInputToDto(MoneyOperationModel moneyOperation)
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

        private MoneyOperationModel MapDtoToViewData(MoneyOperationDto moneyOperationDto)
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

        private MoneyOperationPeriodMetadata ReadCurrentPeriodMetadata(MoneyOperationDto moneyOperationDto, DateTime date)
        {
            MoneyOperationPeriodMetadata metadata = new MoneyOperationPeriodMetadata();

            DateTime currentDate = moneyOperationDto.ValidityBeginDate;
            DateTime nowPeriodBeginningDate = DateTime.UtcNow;
            DateTime nowPeriodEndDate = DateTime.UtcNow;
            DateTime currentPeriodBeginningDate = currentDate;
            short currentPaymentNumber = 0;
            short totalPaymentsNumber = 0;
            if (moneyOperationDto.RepetitionUnitQuantity == 0)
            {
                return null;
            }
            //TODO: Write some helper that would check only to repetitionUnit precision, for example reject minutes and seconds if unit is hour
            while (CompareWithRepetitionUnitPrecision(currentDate, moneyOperationDto.ValidityEndDate, moneyOperationDto.RepetitionUnit, moneyOperationDto.RepetitionUnitQuantity))
            {
                totalPaymentsNumber++;
                if (currentDate <= date)
                {
                    currentPaymentNumber++;
                    currentPeriodBeginningDate = currentDate;
                }
                if (currentDate <= DateTime.UtcNow)
                {
                    nowPeriodBeginningDate = currentDate;
                    nowPeriodEndDate = RepetitionUnitCalculator.CalculateNextRepetitionDate(nowPeriodBeginningDate, moneyOperationDto.RepetitionUnit, moneyOperationDto.RepetitionUnitQuantity);
                    metadata.NowPaymentNumber = totalPaymentsNumber;
                }
                currentDate = RepetitionUnitCalculator.CalculateNextRepetitionDate(currentDate, moneyOperationDto.RepetitionUnit, moneyOperationDto.RepetitionUnitQuantity);
            }

            metadata.CurrentPaymentNumber = currentPaymentNumber;
            metadata.CurrentPeriodBeginningDate = currentPeriodBeginningDate;
            metadata.NowPeriodBeginningDate = nowPeriodBeginningDate;
            metadata.TotalPaymentsNumber = totalPaymentsNumber;

            return metadata;
        }

        private bool CompareWithRepetitionUnitPrecision(DateTime date, DateTime referenceDate, PeriodUnit repetitionUnit, short repetitionUnitQuantity)
        {
            return RepetitionUnitCalculator.ClearMinorDateTimePart(date, repetitionUnit) < RepetitionUnitCalculator.ClearMinorDateTimePart(referenceDate, repetitionUnit);
        }

        /// <summary>
        /// dateFromPeriod is used to calculate the begginning and end date of current period; method returns all changes that occured in this period; should be splitted to at least two methods
        /// </summary>
        /// <param name="moneyOperationDto"></param>
        /// <param name="dateFromPeriod"></param>
        /// <returns></returns>
        private IEnumerable<MoneyOperationChangeDto> ExtractPeriodMoneyOperationChanges(MoneyOperationDto moneyOperationDto, DateTime dateFromPeriod)
        {
            IEnumerable<MoneyOperationChangeDto> periodChanges = new List<MoneyOperationChangeDto>();
            periodChanges = moneyOperationDto.MoneyOperationChanges.Where(moc => ChallengeRepetitionPeriod(dateFromPeriod, moc.ChangeDate, moneyOperationDto.RepetitionUnit, moneyOperationDto.RepetitionUnitQuantity));
            return periodChanges;
        }

        /// <summary>
        /// Returns true if the slaveDate is placed in the repetition period specified by masterDate, repetitionUnit and repetitionUnitQuantity
        /// </summary>
        /// <param name="masterDate"></param>
        /// <param name="slaveDate"></param>
        /// <param name="repetitionUnit"></param>
        /// <param name="repetitionUnitQuantity"></param>
        /// <returns></returns>
        private bool ChallengeRepetitionPeriod(DateTime masterDate, DateTime slaveDate, PeriodUnit repetitionUnit, short repetitionUnitQuantity)
        {
            if (repetitionUnitQuantity == 0)
            {
                repetitionUnitQuantity = 1;
            }
            var periodBeginning = RepetitionUnitCalculator.ClearMinorDateTimePart(masterDate, repetitionUnit);
            var timestampToIncrement = RepetitionUnitCalculator.CalculateRepetitionTimeStamp(masterDate, repetitionUnit, repetitionUnitQuantity);
            var nextPeriodBegining = periodBeginning.Add(timestampToIncrement);

            return nextPeriodBegining > slaveDate && periodBeginning <= slaveDate;
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

    internal class MoneyOperationStatusDatasource
    {
    }
}
