using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinanceManager.Types.Enums;

namespace FinanceManager.BL
{
    //TODO: Refactor the name of this etc.
    public class RepetitionUnitCalculator
    {
        public static DateTime CalculateNextRepetitionDate(DateTime targetDateTime, PeriodUnit repetitionUnit, short repetitionUnitQuantity)
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

        public static TimeSpan CalculateRepetitionTimeStamp(DateTime date, PeriodUnit repetitionUnit, short repetitionUnitQuantity)
        {
            switch (repetitionUnit)
            {
                case PeriodUnit.Day:
                    return new TimeSpan(repetitionUnitQuantity, 0 ,0 ,0);
                case PeriodUnit.Hour:
                    return new TimeSpan(0, repetitionUnitQuantity, 0, 0);
                case PeriodUnit.Minute:
                    return new TimeSpan(0, 0, repetitionUnitQuantity, 0);
                case PeriodUnit.Month:
                    return date.AddMonths(repetitionUnitQuantity) - date;
                case PeriodUnit.Second:
                    return new TimeSpan(0, 0, 0, repetitionUnitQuantity);
                case PeriodUnit.Week:
                    return date.AddDays(repetitionUnitQuantity * 7) - date;
                case PeriodUnit.Year:
                    return date.AddYears(repetitionUnitQuantity) - date;
                case PeriodUnit.Default:
                    return new TimeSpan();
                default:
                    return new TimeSpan();

            }
        }

        public static DateTime ClearMinorDateTimePart(DateTime changeDate, PeriodUnit repetitionUnit, short repetitionUnitQuantity)
        {
            var newDate = changeDate;

            if (repetitionUnit > PeriodUnit.Second) { newDate = newDate.AddSeconds(-newDate.Second); }
            if (repetitionUnit > PeriodUnit.Minute) { newDate = newDate.AddMinutes(-newDate.Minute); }
            if (repetitionUnit > PeriodUnit.Hour) { newDate = newDate.AddHours(-newDate.Hour); }
            if (repetitionUnit > PeriodUnit.Week) { newDate = newDate.AddDays(-newDate.Day + 1); }
            if (repetitionUnit == PeriodUnit.Week) { while (newDate.DayOfWeek != DayOfWeek.Monday) { newDate = newDate.AddDays(-1); } }
            if (repetitionUnit > PeriodUnit.Month) { newDate = newDate.AddMonths(-newDate.Month + 1); }

            return newDate;
        }
    }
}
