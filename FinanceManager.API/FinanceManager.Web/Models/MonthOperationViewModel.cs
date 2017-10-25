using System;

namespace FinanceManager.Web.Models
{

    public class MonthOperationViewModel
    {
        public string Name { get; set; }
        public double TotalAmount { get; set; }
        public double? AlreadyPayedAmount { get; set; }
        public double? PaymentLeftAmount { get { return TotalAmount - AlreadyPayedAmount; } }
        public DateTime FinishDate { get; set; }


    }
}