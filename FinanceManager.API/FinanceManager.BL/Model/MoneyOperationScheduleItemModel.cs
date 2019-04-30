namespace FinanceManager.BL //TODO should I move it away from BL?
{
    public class MoneyOperationScheduleItemModel
    {
        public string PeriodName { get; set; }
        public double CurrentBudgetedAmount { get; set; }
        public double CurrentPayedAmount { get; set; }
        public double TotalAmount { get; set; }
        public double LeftBudgetedAmount => TotalBudgetedAmount - CurrentBudgetedAmount;
        public double TotalBudgetedAmount { get; set; }
    }
}