﻿using System.Collections.Generic;

namespace FinanceManager.Web.Models
{
    //TODO refactor this - create field-label relation and make it more generic
    public class PeriodOperationsViewModel
    {
        public PeriodOperationsViewModel()
        {
            PeriodOperations = new List<MoneyOperationViewModel>();
        }

        public string NameLabel { get; set; }
        public string TotalAmountLabel { get; set; }
        public string AlreadyPayedLabel { get; set; }
        public string PaymentLeftLabel { get; set; }
        public string FinishDateLabel { get; set; }
        public string CurrentPeriodPaymentLeftLabel { get; set; }
        public string CurrentPeriodEndAmountLabel { get; set; }
        public string CurrentPeriodBudgetedAmountLabel { get; set; }
        public string CurrentPeriodPayedAmountLabel { get; set; }
        public IEnumerable<MoneyOperationViewModel> PeriodOperations { get; set; }
    }
}