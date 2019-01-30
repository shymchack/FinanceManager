using Newtonsoft.Json;

namespace FinanceManager.Importer
{
    public class BudgetedCommitment : Commitment
    {
        [JsonProperty]
        public int PaymentMonthNumberColumnIndex { get; set; }
    }
}