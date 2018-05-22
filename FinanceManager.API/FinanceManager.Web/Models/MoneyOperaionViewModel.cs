using System;

namespace FinanceManager.Web.Models
{

    public class MoneyOperationViewModel
    {
        public string Name { get; set; }
        public double InitialAmount { get; set; }
        public double AlreadyPayedAmount { get; set; }
        public double CurrentPeriodPayedAmount { get; set; }
        public double CurrentAmount { get; set; }
        public DateTime FinishDate { get; set; }
        public DateTime BeginningDate { get; set; }
        public double CurrentPeriodPaymentLeft { get; set; }
        public double CurrentPeriodBudgetedAmount { get; set; }
        public double CurrentPeriodEndAmount { get; set; }
        public double CurrentPeriodIncomes { get; set; }
    }
}