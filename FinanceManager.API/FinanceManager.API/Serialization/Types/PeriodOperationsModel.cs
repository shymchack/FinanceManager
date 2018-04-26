using System.Collections.Generic;

namespace FinanceManager.API.Serialization.Types
{
    public class PeriodOperationsModel
    {
        public PeriodOperationsModel()
        {
            PeriodOperations = new List<PeriodOperationModel>();
        }

        public string NameLabel { get; set; }
        public string TotalAmonutLabel { get; set; }
        public string AlreadyPayedLabel { get; set; }
        public string PaymentLeftLabel { get; set; }
        public string FinishDateLabel { get; set; }
        public string CurrentPeriodPayedLabel { get; set; }

        public IEnumerable<PeriodOperationModel> PeriodOperations { get; set; }
    }
}