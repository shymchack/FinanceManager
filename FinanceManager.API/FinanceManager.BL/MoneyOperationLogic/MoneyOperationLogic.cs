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
        private IPeriodicityLogic _periodicityLogic;

        public MoneyOperationLogic(IPeriodicityLogic periodicityLogic)
        {
            _periodicityLogic = periodicityLogic;
        }

        public MoneyOperationDto ConvertUserInputToDto(MoneyOperationModel moneyOperation)
        {
            return MapUserInputToDto(moneyOperation);
        }

        public MoneyOperationModel ConvertDtoToViewData(MoneyOperationDto moneyOperationDto)
        {
            return MapDtoToViewData(moneyOperationDto);
        }

        public MoneyOperationStatusModel PrepareMoneyOperationStatus(MoneyOperationDto moneyOperationDto, PeriodInfo periodInfo)
        {
            var shouldOperationGetAlreadyCleared = periodInfo.BeginDate > moneyOperationDto.ValidityEndDate;
            if (periodInfo.EndDate < moneyOperationDto.ValidityBeginDate) return null; //TODO: maybe should return new()? but this should not happen probably

            var initialAmount = moneyOperationDto.InitialAmount;

            var status = new MoneyOperationStatusModel();
            status.AccountID = moneyOperationDto.AccountID;
            status.Description = moneyOperationDto.Description;
            status.Name = moneyOperationDto.Name;
            status.FinishDate = moneyOperationDto.ValidityEndDate;
            status.BeginningDate = moneyOperationDto.ValidityBeginDate;
            status.InitialAmount = initialAmount;

            var periodMetadata = ReadPeriodMetadata(moneyOperationDto, periodInfo);
            var moneyOperationChanges = ExtractMoneyOperationChanges(moneyOperationDto, periodInfo);
            var currentPeriodOperationChanges = ExtractPeriodMoneyOperationChanges(moneyOperationChanges, periodInfo);
            //Yes, this should track also incomes, as we assume the income is just for purpose of modifying an outcome (for example when the price has changed)
            var currentPeriodPayedAmount = currentPeriodOperationChanges.Sum(moc => -moc.ChangeAmount);

            decimal periodBeginningAmountCandidate = initialAmount;
            decimal alreadyPayedAmountBeforeCurrentCandidate = 0;
            decimal currentAmountCandidate = initialAmount;
            decimal currentPeriodBudgetedAmountCandidate = 0;
            //TODO refactoring!
            var periodsLeftToPayIncludingNow = periodMetadata.TotalPaymentsNumber - periodMetadata.NowPaymentNumber + 1;
            alreadyPayedAmountBeforeCurrentCandidate = moneyOperationDto.MoneyOperationChanges.Where(moc => moc.ChangeDate < periodInfo.BeginDate && moc.ChangeDate >= moneyOperationDto.ValidityBeginDate).Sum(moc => -moc.ChangeAmount);
            periodBeginningAmountCandidate -= alreadyPayedAmountBeforeCurrentCandidate;
            //TODO should I round the date up to end of current period? Think about periodMetadata.NowPaymentNumber usage below
            currentAmountCandidate -= (alreadyPayedAmountBeforeCurrentCandidate + currentPeriodPayedAmount);
            currentPeriodBudgetedAmountCandidate = periodsLeftToPayIncludingNow > 0 ? Math.Max(0, (periodBeginningAmountCandidate / periodsLeftToPayIncludingNow)) : 0;
            currentPeriodBudgetedAmountCandidate -= currentPeriodPayedAmount;
            decimal currentPeriodEndAmountCandidate = currentAmountCandidate;
            currentPeriodEndAmountCandidate -= currentPeriodBudgetedAmountCandidate;

            var operationAlreadyCleared = currentAmountCandidate == 0;

            if (shouldOperationGetAlreadyCleared && operationAlreadyCleared)
                return null;

            status.CurrentAmount = currentAmountCandidate;
            status.AlreadyPayedAmount = alreadyPayedAmountBeforeCurrentCandidate + currentPeriodPayedAmount;
            status.CurrentPeriodPayedAmount = currentPeriodPayedAmount;
            status.CurrentPeriodBudgetedAmount = currentPeriodBudgetedAmountCandidate;
            status.CurrentPeriodEndAmount = currentPeriodEndAmountCandidate;

            return status;
        }

        private IEnumerable<MoneyOperationChangeDto> ExtractPeriodMoneyOperationChanges(IEnumerable<MoneyOperationChangeDto> moneyOperationChanges, PeriodInfo periodInfo)
        {
            return moneyOperationChanges.Where(moc => moc.ChangeDate >= periodInfo.BeginDate);
        }

        public MoneyOperationScheduleModel GetMoneyOperationSchedule(MoneyOperationDto moneyOperationDto)
        {
            MoneyOperationScheduleModel moneyOperationScheduleModel = new MoneyOperationScheduleModel();

            moneyOperationScheduleModel.Name = moneyOperationDto.Name;
            moneyOperationScheduleModel.TotalAmount = (double)(moneyOperationDto.InitialAmount + moneyOperationDto.MoneyOperationChanges.Sum(moc => -moc.ChangeAmount));
            moneyOperationScheduleModel.InitialAmount = (double)moneyOperationDto.InitialAmount;
            DateTime date = moneyOperationDto.ValidityBeginDate;
            var scheduleItems = new List<MoneyOperationScheduleItemModel>();
            var totalBudgetedAmount = 0d;
            while (date <= moneyOperationDto.ValidityEndDate)
            {
                var periodInfo = _periodicityLogic.GetPeriodInfo(date, moneyOperationDto.RepetitionUnit);
                var moneyOperationStatus = PrepareMoneyOperationStatus(moneyOperationDto, periodInfo);
                var itemBalance = (double)moneyOperationStatus.CurrentAmount;
                var budgetedAmount = Math.Max(0, (double)moneyOperationStatus.CurrentPeriodBudgetedAmount);
                var payedAmount = (double)moneyOperationStatus.CurrentPeriodPayedAmount;
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

        private MoneyOperationPeriodMetadata ReadPeriodMetadata(MoneyOperationDto moneyOperationDto, PeriodInfo periodInfo)
        {
            MoneyOperationPeriodMetadata metadata = new MoneyOperationPeriodMetadata();

            //no budgeted amount - no period metadata
            if (moneyOperationDto.MoneyOperationSetting.ReservationPeriodQuantity == 0)
            {
                return metadata;
            }

            DateTime currentDate = moneyOperationDto.ValidityBeginDate;
            short currentPaymentNumber = 0;
            short totalPaymentsNumber = 0;
            //TODO: Write some helper that would check only to repetitionUnit precision, for example reject minutes and seconds if unit is hour
            while (currentDate <= moneyOperationDto.ValidityEndDate)
            {
                totalPaymentsNumber++;
                if (currentDate <= periodInfo.EndDate)
                {
                    currentPaymentNumber++;
                    metadata.NowPaymentNumber = totalPaymentsNumber;
                }
                //TODO fix this short casting shit
                currentDate = RepetitionUnitCalculator.CalculateNextRepetitionDate(currentDate, moneyOperationDto.MoneyOperationSetting.ReservationPeriodUnit);
            }

            metadata.CurrentPaymentNumber = currentPaymentNumber;
            metadata.TotalPaymentsNumber = totalPaymentsNumber;

            return metadata;
        }

        private bool CompareWithRepetitionUnitPrecision(DateTime date, MoneyOperationDto moneyOperationDto)
        {
            return RepetitionUnitCalculator.ClearMinorDateTimePart(date, moneyOperationDto.MoneyOperationSetting.ReservationPeriodUnit) < RepetitionUnitCalculator.ClearMinorDateTimePart(moneyOperationDto.ValidityEndDate, moneyOperationDto.MoneyOperationSetting.ReservationPeriodUnit);
        }

        /// <summary>
        /// targetDate is used to calculate the begginning and end date of current period; method returns all changes that occured in this period; should be splitted to at least two methods
        /// </summary>
        /// <param name="moneyOperationDto"></param>
        /// <param name="targetDate"></param>
        /// <returns></returns>
        private IEnumerable<MoneyOperationChangeDto> ExtractMoneyOperationChanges(MoneyOperationDto moneyOperationDto, PeriodInfo periodInfo)
        {
            IEnumerable<MoneyOperationChangeDto> periodChanges = new List<MoneyOperationChangeDto>();
            periodChanges = moneyOperationDto.MoneyOperationChanges.Where(moc => moc.ChangeDate <= periodInfo.EndDate);
            return periodChanges;
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

            while (nextExecutionDateCandidate < DateTime.UtcNow)
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

        public bool IsOperationBudgeted(MoneyOperationDto mo)
        {
            //TODO think about making operation that is budgeted and cyclic at once. Remember to not break only cyclic operations
            return (mo.MoneyOperationSetting?.ReservationPeriodQuantity ?? 0) > 0 && mo.RepetitionUnitQuantity == 1;
        }

        public bool IsOperationCyclic(MoneyOperationDto mo)
        {
            return mo.MoneyOperationSetting?.ReservationPeriodQuantity == 0 && mo.RepetitionUnitQuantity > 0;
        }

        public bool IsOperationSingle(MoneyOperationDto mo)
        {
            return mo.MoneyOperationSetting == null && mo.RepetitionUnitQuantity == 0;
        }
    }
}
