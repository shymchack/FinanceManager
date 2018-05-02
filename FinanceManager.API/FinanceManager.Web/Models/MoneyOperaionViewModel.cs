using System;

namespace FinanceManager.Web.Models
{

    public class MoneyOperaionViewModel
    {
        public string Name { get; set; }
        public decimal InitialAmount { get; set; }
        public decimal AlreadyPayedAmount { get; set; }
        public decimal CurrentPeriodPayedAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public DateTime FinishDate { get; set; }
        public DateTime BeginningDate { get; set; }
    }
}