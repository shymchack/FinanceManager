using System.Collections.Generic;

namespace FinanceManager.BL //TODO should I move it away from BL?
{
    public class MoneyOperationScheduleModel
    {
        public MoneyOperationScheduleModel()
        {
            ScheduleItem = new List<MoneyOperationScheduleItemModel>();
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
        public IEnumerable<MoneyOperationScheduleItemModel> ScheduleItem { get; set; }
    }
}