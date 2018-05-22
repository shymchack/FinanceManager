using System.Collections.Generic;

namespace FinanceManager.Web.Models
{
    public class MoneyOperationScheduleViewModel
    {
        public MoneyOperationScheduleViewModel()
        {
            ScheduleItem = new List<MoneyOperationScheduleItemViewModel>();
        }

        public string Name { get; set; }
        public double TotalAmount { get; set; }
        public double PaymentLeft { get; set; }
        public IEnumerable<MoneyOperationScheduleItemViewModel> ScheduleItem { get; set; }
    }
}