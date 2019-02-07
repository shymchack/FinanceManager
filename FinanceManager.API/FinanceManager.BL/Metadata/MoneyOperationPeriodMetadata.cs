using System;

namespace FinanceManager.BL.Metadata
{
    public class MoneyOperationPeriodMetadata
    {
        public int CurrentPaymentNumber { get; internal set; }
        public short NowPaymentNumber { get; internal set; }
        public int TotalPaymentsNumber { get; internal set; }
    }
}