﻿using FinanceManager.BL;
using System.Collections.Generic;

namespace FinanceManager.API.Serialization.Types
{
    public class PeriodOperationsModel
    {
        public PeriodOperationsModel()
        {
            PeriodOperations = new List<MoneyOperationStatus>();
        }

        public string NameLabel { get; set; }
        public string TotalAmonutLabel { get; set; }
        public string AlreadyPayedLabel { get; set; }
        public string PaymentLeftLabel { get; set; }
        public string FinishDateLabel { get; set; }
        public string CurrentPeriodPayedLabel { get; set; }
        public string TotalBudgetedAmountLabel { get; set; }
        public string CurrentPeriodBeginningBudgetedAmountLabel { get; set; }
        public string CurrentPeriodBudgetedAmountLabel { get; set; }
        public string CurrentPeriodEndBudgetedAmountLabel { get; set; }

        public IEnumerable<MoneyOperationStatus> PeriodOperations { get; set; }
    }
}