using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.BL.UserInput
{
    public class MoneyOperationChangeModel
    {
        public int MoneyOperationId { get; set; }
        public decimal ChangeAmount { get; set; }
        public DateTime ChangeDate { get; set; }
        public string Description { get; set; }
    }
}
