using System;
using FinanceManager.Types.Enums;

namespace FinanceManager.BL
{
    public class RepetitionUnitCalculator
    {
        public static DateTime CalculateNextRepetitionDate(DateTime targetDateTime, PeriodUnit repetitionUnit, short repetitionUnitQuantity = 1)
        {
            //using constructor is for preventing any ticks and milliseconds being inherited from changeDate
            var newDate = new DateTime(targetDateTime.Year, targetDateTime.Month, targetDateTime.Day, targetDateTime.Hour, targetDateTime.Minute, targetDateTime.Second);

            switch (repetitionUnit)
            {
                case PeriodUnit.Day:
                    return newDate.AddDays(repetitionUnitQuantity);
                case PeriodUnit.Hour:
                    return newDate.AddHours(repetitionUnitQuantity);
                case PeriodUnit.Minute:
                    return newDate.AddMinutes(repetitionUnitQuantity);
                case PeriodUnit.Month:
                    return newDate.AddMonths(repetitionUnitQuantity);
                case PeriodUnit.Second:
                    return newDate.AddSeconds(repetitionUnitQuantity);
                case PeriodUnit.Week:
                    return newDate.AddDays(7 * repetitionUnitQuantity);
                case PeriodUnit.Year:
                    return newDate.AddYears(repetitionUnitQuantity);
                default:
                    return newDate;

            }
        }

        public static TimeSpan CalculateRepetitionTimeStamp(DateTime date, PeriodUnit repetitionUnit, short repetitionUnitQuantity)
        {
            //TODO extract method for gathering this
            //using constructor is for preventing any ticks and milliseconds being inherited from changeDate
            var newDate = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);

            switch (repetitionUnit)
            {
                case PeriodUnit.Day:
                    return new TimeSpan(repetitionUnitQuantity, 0 ,0 ,0);
                case PeriodUnit.Hour:
                    return new TimeSpan(0, repetitionUnitQuantity, 0, 0);
                case PeriodUnit.Minute:
                    return new TimeSpan(0, 0, repetitionUnitQuantity, 0);
                case PeriodUnit.Month:
                    return newDate.AddMonths(repetitionUnitQuantity) - newDate;
                case PeriodUnit.Second:
                    return new TimeSpan(0, 0, 0, repetitionUnitQuantity);
                case PeriodUnit.Week:
                    return newDate.AddDays(repetitionUnitQuantity * 7) - newDate;
                case PeriodUnit.Year:
                    return newDate.AddYears(repetitionUnitQuantity) - newDate;
                default:
                    return new TimeSpan();

            }
        }

        public static DateTime ClearMinorDateTimePart(DateTime changeDate, PeriodUnit repetitionUnit)
        {
            //using constructor is for preventing any ticks and milliseconds being inherited from changeDate
            var newDate = new DateTime(changeDate.Year, changeDate.Month, changeDate.Day, changeDate.Hour, changeDate.Minute, changeDate.Second);

            if (repetitionUnit > PeriodUnit.Second) { newDate = newDate.AddSeconds(-newDate.Second); }
            if (repetitionUnit > PeriodUnit.Minute) { newDate = newDate.AddMinutes(-newDate.Minute); }
            if (repetitionUnit > PeriodUnit.Hour) { newDate = newDate.AddHours(-newDate.Hour); }
            if (repetitionUnit > PeriodUnit.Day) { newDate = newDate.AddDays(-newDate.Day + 1); }
            if (repetitionUnit == PeriodUnit.Week) { while (newDate.DayOfWeek != DayOfWeek.Monday) { newDate = newDate.AddDays(-1); } }
            if (repetitionUnit > PeriodUnit.Month) { newDate = newDate.AddMonths(-newDate.Month + 1); }

            return newDate;
        }
    }
}
