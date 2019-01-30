using Newtonsoft.Json;
using System.Collections.Generic;

namespace FinanceManager.Importer
{
    [JsonObject]
    public class ExcelFolder
    {
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public List<Sheet> Sheets { get; set; }
    }
}