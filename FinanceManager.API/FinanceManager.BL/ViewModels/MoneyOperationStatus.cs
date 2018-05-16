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
        public decimal CurrentPeriodPaymentLeft { get { return Math.Max(0, CurrentPeriodBudgetedAmount - CurrentPeriodPayedAmount); } }
        public decimal CurrentPeriodBudgetedAmount { get { return Math.Max(0, CurrentAmount / PeriodsLeftToPay); } }
        public decimal CurrentPeriodEndAmount { get { return CurrentAmount - CurrentPeriodBudgetedAmount; } } 
        public decimal CurrentPeriodIncomes { get; set; }
        public int PeriodsLeftToPay { get; set; }
    }
}