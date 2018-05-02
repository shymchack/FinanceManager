using System;
using FinanceManager.Types.Enums;

namespace FinanceManager.BL
{
    public class MoneyOperationStatus
    {
        public string Name { get; set; }
        public decimal InitialAmount { get; set; }
        public decimal AlreadyPayedAmount { get; set; }
        public decimal CurrentPeriodPayedAmount { get; set; }
        public decimal CurrentAmount { get { return InitialAmount - AlreadyPayedAmount; } }
        public DateTime FinishDate { get; set; }
        public DateTime BeginningDate { get; set; }

        public string Description { get; set; }
        public short RepetitionUnitQuantity { get; set; } //TODO: Rename
        public PeriodUnit RepetitionUnit { get; set; } //TODO: Rename
        public int AccountID { get; set; }
        public decimal TotalBudgetedAmount { get; set; }
        public decimal CurrentPeriodBudgetedAmount { get { return CurrentPeriodBeginningBudgetedAmount - CurrentPeriodPayedAmount; } }
        public decimal CurrentPeriodBeginningBudgetedAmount { get { return TotalBudgetedAmount / PeriodsLeftToPay; } }
        public decimal CurrentPeriodEndBudgetedAmount { get { return TotalBudgetedAmount / (PeriodsLeftToPay + 1); } } //TODO verify the count of periods, also in beginning amount
        public decimal CurrentPeriodIncomes { get; set; }
        public int PeriodsLeftToPay { get; set; }
    }
}