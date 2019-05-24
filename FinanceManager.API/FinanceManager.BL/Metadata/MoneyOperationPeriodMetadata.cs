using System;

namespace FinanceManager.BL.Metadata
{
    public class MoneyOperationPeriodMetadata
    {
        public short NowPaymentNumber { get; internal set; }
        public int TotalPaymentsNumber { get; internal set; }
    }
}