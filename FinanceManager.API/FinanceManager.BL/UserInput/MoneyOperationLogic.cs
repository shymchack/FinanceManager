using FinanceManager.DAL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinanceManager.Types.Enums;
using System.Globalization;

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
            if (date < moneyOperationDto.ValidityBeginDate || date > moneyOperationDto.ValidityEndDate) return null; //TODO: maybe should return new()?
            DateTime currentDate = moneyOperationDto.ValidityBeginDate;
            DateTime currentPeriodBeginningDate = currentDate;
            short currentPaymentNumber = 1;
            short totalPaymentsNumber = 1;
            //TODO: Write some helper that would check only to repetitionUnit precision, for example reject minutes and seconds if unit is hour
            while(currentDate <= moneyOperationDto.ValidityEndDate)
            {
                if (currentDate <= date)
                {
                    currentPaymentNumber++;
                    currentPeriodBeginningDate = currentDate;
                }
                totalPaymentsNumber++;
                currentDate = RepetitionUnitCalculator.CalculateNextRepetitionDate(currentDate, moneyOperationDto.RepetitionUnit, moneyOperationDto.RepetitionUnitQuantity);
            }

            //TimeSpan repetitionTimeSpan = RepetitionUnitCalculator.CalculateRepetitionTimeSpan(moneyOperationDto); // I don't know if this is necessary
            IEnumerable<MoneyOperationChangeDto> periodMoneyOperationChanges = ExtractPeriodOperations(moneyOperationDto, date);
            var periodsLeftToPay = totalPaymentsNumber - currentPaymentNumber;
            var alreadyPayedAmount = moneyOperationDto.MoneyOperationChanges.Where(moc => moc.ChangeDate < currentPeriodBeginningDate).Sum(moc => -moc.ChangeAmount);
            var initialAmount = moneyOperationDto.InitialAmount;
            var currentPeriodPayedAmount = periodMoneyOperationChanges.Sum(moc => -moc.ChangeAmount);
            var currentAmount = initialAmount - alreadyPayedAmount - currentPeriodPayedAmount;
            var currentPeriodBudgetedAmount = periodsLeftToPay > 0 ? Math.Max(0, (currentAmount + currentPeriodPayedAmount) / periodsLeftToPay) : 0;
            var currentPeriodPaymentLeft = Math.Max(0, currentPeriodBudgetedAmount - currentPeriodPayedAmount);
            var currentPeriodEndAmount = currentAmount - currentPeriodPaymentLeft;

            MoneyOperationStatus status = new MoneyOperationStatus();
            status.AccountID = moneyOperationDto.AccountID;
            status.Description = moneyOperationDto.Description;
            status.Name = moneyOperationDto.Name;
            status.FinishDate = moneyOperationDto.ValidityEndDate;
            status.BeginningDate = moneyOperationDto.ValidityBeginDate;

            status.InitialAmount = initialAmount;
            status.CurrentAmount = currentAmount;
            status.AlreadyPayedAmount = alreadyPayedAmount;
            status.CurrentPeriodPayedAmount = currentPeriodPayedAmount;
            status.CurrentPeriodBudgetedAmount = currentPeriodBudgetedAmount;
            status.CurrentPeriodPaymentLeft = currentPeriodPaymentLeft;
            status.CurrentPeriodEndAmount = currentPeriodEndAmount;
            //status.TotalBudgetedAmount = (decimal)currentPaymentNumber / (decimal)totalPaymentsNumber * (status.InitialAmount - status.AlreadyPayedAmount); // TODO: Make sure it's needed to subtract already payed amount

            //TODO at first implement the "money operation freeze feature" - remember to rename FrozenAmount to prevent misunderstaindings.
            //status.CurrPeriodIncomes = moneyOperationChanges.Where(mo => mo.ChangeAmount > 0).Sum(moc => moc.ChangeAmount);

            return status;
        }

        public MoneyOperationScheduleModel GetMoneyOperationSchedule(MoneyOperationDto moneyOperationDto)
        {
            MoneyOperationScheduleModel moneyOperationScheduleModel = new MoneyOperationScheduleModel();

            moneyOperationScheduleModel.Name = moneyOperationDto.Name;
            moneyOperationScheduleModel.TotalAmount= moneyOperationDto.InitialAmount + moneyOperationDto.MoneyOperationChanges.Sum(moc => -moc.ChangeAmount); // TODO make InitialAmount not focused on expenses only. Invert the value.
            DateTime date = moneyOperationDto.ValidityBeginDate;
            var scheduleItems = new List<MoneyOperationScheduleItemModel>();
            while (date <= moneyOperationDto.ValidityEndDate)
            {
                var moneyOperationStatus = PrepareMoneyOperationStatus(moneyOperationDto, date);
                MoneyOperationScheduleItemModel scheduleItem = new MoneyOperationScheduleItemModel();
                scheduleItem.ItemBalance = (double)moneyOperationStatus.CurrentAmount;
                scheduleItem.BudgetedAmount = (double) moneyOperationStatus.CurrentPeriodBudgetedAmount;
                scheduleItem.PayedAmount = (double)moneyOperationStatus.CurrentPeriodPayedAmount;
                scheduleItem.PeriodName = date.ToString("MMMM", CultureInfo.InvariantCulture); // TODO this is so bad to use month name. Remember that not only months can be processed.
                date = RepetitionUnitCalculator.CalculateNextRepetitionDate(date, moneyOperationDto.RepetitionUnit, moneyOperationDto.RepetitionUnitQuantity);

                scheduleItems.Add(scheduleItem);
            }
            moneyOperationScheduleModel.ScheduleItem = scheduleItems;
            return moneyOperationScheduleModel;
        }

        private IEnumerable<MoneyOperationChangeDto> ExtractPeriodOperations(MoneyOperationDto moneyOperationDto, DateTime dateFromPeriod)
        {
            var periodChanges = moneyOperationDto.MoneyOperationChanges.Where(moc => ChallengeRepetitionPeriod(dateFromPeriod, moc.ChangeDate, moneyOperationDto.RepetitionUnit, moneyOperationDto.RepetitionUnitQuantity));
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
            var periodBeginning = RepetitionUnitCalculator.ClearMinorDateTimePart(masterDate, repetitionUnit, repetitionUnitQuantity);
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
}
