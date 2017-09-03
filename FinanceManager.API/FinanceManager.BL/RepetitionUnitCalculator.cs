using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinanceManager.Types.Enums;

namespace FinanceManager.BD
{
    //TODO: Refactor the name of this etc.
    public class RepetitionUnitCalculator
    {
        public DateTime CalculateNextRepetitionDate(DateTime targetDateTime, PeriodUnit repetitionUnit, short repetitionUnitQuantity)
        {
            switch (repetitionUnit)
            {
                case PeriodUnit.Day:
                    return targetDateTime.AddDays(repetitionUnitQuantity);
                case PeriodUnit.Hour:
                    return targetDateTime.AddHours(repetitionUnitQuantity);
                case PeriodUnit.Minute:
                    return targetDateTime.AddMinutes(repetitionUnitQuantity);
                case PeriodUnit.Month:
                    return targetDateTime.AddMonths(repetitionUnitQuantity);
                case PeriodUnit.Second:
                    return targetDateTime.AddSeconds(repetitionUnitQuantity);
                case PeriodUnit.Week:
                    return targetDateTime.AddDays(7 * repetitionUnitQuantity);
                case PeriodUnit.Year:
                    return targetDateTime.AddYears(repetitionUnitQuantity);
                case PeriodUnit.Default:
                    return targetDateTime;
                default:
                    return targetDateTime;

            }
        }
    }
}
