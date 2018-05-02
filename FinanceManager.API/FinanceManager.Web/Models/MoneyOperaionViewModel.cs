using System;

namespace FinanceManager.Web.Models
{

    public class MoneyOperaionViewModel
    {
        public string Name { get; set; }
        public double InitialAmount { get; set; }
        public double AlreadyPayedAmount { get; set; }
        public double CurrentPeriodPayedAmount { get; set; }
        public double CurrentAmount { get; set; }
        public DateTime FinishDate { get; set; }
        public DateTime BeginningDate { get; set; }
        public double TotalBudgetedAmount { get; set; }
        public double CurrentPeriodBudgetedAmount { get; set; }
        public double CurrentPeriodBeginningBudgetedAmount { get; set; }
        public double CurrentPeriodEndBudgetedAmount { get; set; }
        public double CurrentPeriodIncomes { get; set; }
    }
}