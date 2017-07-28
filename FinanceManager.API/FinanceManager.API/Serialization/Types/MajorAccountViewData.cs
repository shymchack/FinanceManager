using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinanceManager.API.Serialization.Types
{
    public class MajorAccountViewData
    {
        public string Name { get; set; }
        public string CurrencyCode { get; set; }
        public List<AccountOperation> Expenses { get; set; }
        public List<AccountOperation> Incomes { get; set; }
        public AccountSummary Summary { get; set; }
    }
}