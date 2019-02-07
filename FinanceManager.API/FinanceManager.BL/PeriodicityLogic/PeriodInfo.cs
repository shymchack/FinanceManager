using FinanceManager.Types.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.BL
{
    public class PeriodInfo
    {
        public PeriodInfo(DateTime periodBeginDate, DateTime periodEndDate, PeriodUnit periodUnit)
        {
            BeginDate = periodBeginDate;
            EndDate = periodEndDate;
            PeriodUnit = periodUnit;
        }

        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public PeriodUnit PeriodUnit { get; set; }
    }
}
