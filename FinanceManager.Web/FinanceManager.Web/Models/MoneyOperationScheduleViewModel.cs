using System.Collections.Generic;

namespace FinanceManager.Web.Models
{
    public class MoneyOperationScheduleViewModel
    {
        public MoneyOperationScheduleViewModel()
        {
            ScheduleItem = new List<MoneyOperationScheduleItemViewModel>();
        }

        public string NameLabel { get; set; }
        public string TotalAmountLabel { get; set; }
        public string PaymentLeftLabel { get; set; }
        public string PeriodNameLabel { get; set; }
        public string ItemBalanceLabel { get; set; }
        public string PayedAmountLabel { get; set; }
        public string BudgetedAmountLabel { get; set; }
        public string TotalBudgetedAmountLabel { get; set; }
        public string ItemBudgetedBalanceLabel { get; set; }

        public string Name { get; set; }
        public double InitialAmount { get; set; }
        public double TotalAmount { get; set; }
        public double PaymentLeft { get; set; }
        public IEnumerable<MoneyOperationScheduleItemViewModel> ScheduleItem { get; set; }
    }
}