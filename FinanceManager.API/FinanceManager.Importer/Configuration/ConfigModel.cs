using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Importer
{
    [JsonObject]
    public class ConfigModel
    {
        [JsonProperty]
        public ExcelFolder ExcelFolder { get; set; }
        
    }
}
