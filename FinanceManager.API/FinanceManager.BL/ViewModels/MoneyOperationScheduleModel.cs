using System.Collections.Generic;

namespace FinanceManager.BL //TODO should I move it away from BL?
{
    public class MoneyOperationScheduleModel
    {
        public MoneyOperationScheduleModel()
        {
            ScheduleItem = new List<MoneyOperationScheduleItemModel>();
        }

        public string Name { get; set; }
        public decimal TotalAmount { get; set; }
        public IEnumerable<MoneyOperationScheduleItemModel> ScheduleItem { get; set; }
    }
}