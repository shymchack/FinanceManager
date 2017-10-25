using System.Collections.Generic;

namespace FinanceManager.Web.Models
{
    public class MonthOperationsTableViewModel
    {
        public string NameLabel { get; set; }
        public string TotalAmonutLabel { get; set; }
        public string AlreadyPayedLabel { get; set; }
        public string PaymentLeftLabel { get; set; }
        public string FinishDateLabel { get; set; }

        public IEnumerable<MonthOperationViewModel> MonthOperations { get; set; }
    }
}