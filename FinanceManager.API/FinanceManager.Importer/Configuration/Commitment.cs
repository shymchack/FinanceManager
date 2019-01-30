using Newtonsoft.Json;

namespace FinanceManager.Importer
{
    [JsonObject]
    public class Commitment
    {
        [JsonProperty]
        public int NameColumnIndex { get; set; }
        [JsonProperty]
        public int FirstRowIndex { get; set; }
        [JsonProperty]
        public int LastRowIndex { get; set; }
        [JsonProperty]
        public int TotalAmountColumnIndex { get; set; }
        [JsonProperty]
        public int PayedAmountColumnIndex { get; set; }
        [JsonProperty]
        public int ThisMonthPayedAmountColumnIndex { get; set; }
    }
}
