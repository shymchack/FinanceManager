using System.Collections.Generic;

namespace FinanceManager.API.Serialization.Types
{
    public class MonthOperationsTableViewModel
    {
        public string NameLabel { get; set; }
        public string TotalAmonutLabel { get; set; }
        public string AlreadyPayedLabel { get; set; }
        public string PaymentLeftLabel { get; set; }
        public string FinishDateLabel { get; set; }
        public string CurrentPeriodPayedLabel { get; set; }

        public IEnumerable<MonthOperationViewModel> MonthOperations { get; set; }
    }
}