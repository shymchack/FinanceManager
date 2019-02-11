using System;
using FinanceManager.Types.Enums;

namespace FinanceManager.BL
{
    public class PeriodicityLogic : IPeriodicityLogic
    {
        public PeriodInfo GetPeriodInfo(DateTime targetDate, PeriodUnit periodUnit)
        {
            var periodBeginDate = RepetitionUnitCalculator.ClearMinorDateTimePart(targetDate, periodUnit);
            var periodEndDate = RepetitionUnitCalculator.CalculateNextRepetitionDate(periodBeginDate, periodUnit, 1).AddSeconds(-1);
            
            return new PeriodInfo(periodBeginDate, periodEndDate, periodUnit);
        }

        public bool TestBudgetedOperationFeatures(PeriodUnit repetitionUnit, short repetitionUnitQuantity, PeriodUnit reservationUnit, int reservationUnitQuantity, PeriodInfo periodInfo)
        {
            var repetitionTimeStamp = RepetitionUnitCalculator.CalculateRepetitionTimeStamp(periodInfo.BeginDate, repetitionUnit, repetitionUnitQuantity);
            var reservationTimeStamp = RepetitionUnitCalculator.CalculateRepetitionTimeStamp(periodInfo.BeginDate, reservationUnit, (short)reservationUnitQuantity);

            var result = reservationTimeStamp.TotalMilliseconds >= repetitionTimeStamp.TotalMilliseconds * 2;

            //Sprawdzić czy jest możliwe repetition w periodInfo i w reservation timestampie. Jeśli nie 

            return result;
        }
    }
}
