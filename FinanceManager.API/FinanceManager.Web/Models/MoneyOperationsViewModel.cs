﻿using System.Collections.Generic;

namespace FinanceManager.Web.Models
{
    //TODO refactor this - create field-label relation and make it more generic
    public class MoneyOperationsViewModel
    {
        public MoneyOperationsViewModel()
        {
            MonthOperations = new List<MoneyOperaionViewModel>();
        }

        public string NameLabel { get; set; }
        public string TotalAmonutLabel { get; set; }
        public string AlreadyPayedLabel { get; set; }
        public string PaymentLeftLabel { get; set; }
        public string FinishDateLabel { get; set; }
        public string CurrentPeriodPayedLabel { get; set; }

        public IEnumerable<MoneyOperaionViewModel> MonthOperations { get; set; }
    }
}