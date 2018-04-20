using System;

namespace FinanceManager.Web.Models
{

    public class MonthOperationViewModel
    {
        public string Name { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? AlreadyPayedAmount { get; set; }
        public decimal? PaymentLeftAmount { get { return TotalAmount - AlreadyPayedAmount; } }
        public DateTime FinishDate { get; set; }


    }
}