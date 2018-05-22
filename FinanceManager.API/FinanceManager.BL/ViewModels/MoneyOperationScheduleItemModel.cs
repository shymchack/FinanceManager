namespace FinanceManager.BL //TODO should I move it away from BL?
{
    public class MoneyOperationScheduleItemModel
    {
        public string PeriodName { get; set; }
        public double BudgetedAmount { get; set; }
        public double PayedAmount { get; set; }
        public double ItemBalance { get; set; }
    }
}