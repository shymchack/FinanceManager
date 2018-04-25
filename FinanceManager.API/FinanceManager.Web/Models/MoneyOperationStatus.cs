using System;
using FinanceManager.Types.Enums;

namespace FinanceManager.Web.Models
{
    public class MoneyOperationStatusViewModel
    {
        public decimal InitialAmount { get; set; }
        public decimal FrozenAmount { get; set; }
        public decimal AlreadyPayedAmount { get; set; }
        public decimal CurrentPeriodPayedAmount { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public short RepetitionUnitQuantity { get; set; } //TODO: Rename
        public PeriodUnit RepetitionUnit { get; set; } //TODO: Rename
        public int AccountID { get; set; }
        public decimal CurrentAmount { get { return InitialAmount - AlreadyPayedAmount; } }

        public DateTime FinishDate { get; set; }
        public DateTime BeginningDate { get; set; }
    }
}