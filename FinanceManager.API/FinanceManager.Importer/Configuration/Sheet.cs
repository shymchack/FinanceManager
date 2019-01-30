using Newtonsoft.Json;
using System.Collections.Generic;

namespace FinanceManager.Importer
{
    public class Sheet
    {
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public string BeginDate { get; set; }
        [JsonProperty]
        public string EndDate { get; set; }
        [JsonProperty]
        public List<SingleCommitment> SingleCommitments { get; set; }
        [JsonProperty]
        public List<BudgetedCommitment> BudgetedCommitments { get; set; }
        [JsonProperty]
        public List<CyclicCommitment> CyclicCommitments { get; set; }
        [JsonProperty]
        public List<SingleCommitment> Incomes { get; set; }
        [JsonProperty]
        public int CurrentMonthRowIndex { get; set; }
        [JsonProperty]
        public int CurrentMonthColumnIndex { get; set; }
    }
}