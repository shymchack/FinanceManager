using FinanceManager.Types.Enums;
using System;

namespace FinanceManager.BL
{
    public interface IPeriodicityLogic
    {
        PeriodInfo GetPeriodInfo(DateTime targetDate, PeriodUnit periodUnit);
        bool TestBudgetedOperationFeatures(PeriodUnit repetitionUnit, short repetitionUnitQuantity, PeriodUnit reservationPeriodUnit, int reservationPeriodQuantity, PeriodInfo periodInfo);
    }
}
