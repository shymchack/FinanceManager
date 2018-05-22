﻿namespace FinanceManager.Web.Models
{
    public class MoneyOperationScheduleItemViewModel
    {
        public string PeriodName { get; set; }
        public double BudgetedAmount { get; set; }
        public double PayedAmount { get; set; }
        public double ItemBalance { get; set; }
    }
}