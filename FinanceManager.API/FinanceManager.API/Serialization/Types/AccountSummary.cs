using System.Collections.Generic;

namespace FinanceManager.API.Serialization.Types
{
    public class AccountSummary
    {
        public AccountBalance CurrentBalance { get; set; }
        public List<AccountBalance> PeriodBalance { get; set; }

    }
}