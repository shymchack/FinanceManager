using FinanceManager.Types.Enums;
using System;

namespace FinanceManager.BD.UserInput
{
    public class MoneyOperationViewData
    {
        public int OperationSettingID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal InitialAmount { get; set; }
        public bool IsReal { get; set; }
        public bool IsActive { get; set; }
        public DateTime ValidityBeginDate { get; set; }
        public DateTime ValidityEndDate { get; set; }
        public DateTime NextOperationExecutionDate { get; } //TODO: Decide when to set this prop (not sure if BL)
        public short RepetitionUnitQuantity { get; set; } //TODO: Rename
        public PeriodUnit RepetitionUnit { get; set; } //TODO: Rename
        public int AccountID { get; set; }
    }
}