using System;
using FinanceManager.Types.Enums;

namespace FinanceManager.BL
{
    public class MoneyOperationStatus
    {
        public decimal InitialAmount { get; set; }
        public decimal FrozenAmount { get; set; }
        public decimal AlreadyPayedAmount { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public short RepetitionUnitQuantity { get; set; } //TODO: Rename
        public PeriodUnit RepetitionUnit { get; set; } //TODO: Rename
        public int AccountID { get; set; }
        public decimal CurrentAmount { get { return InitialAmount - AlreadyPayedAmount; } }

        public DateTime FinishDate { get; internal set; }
    }
}