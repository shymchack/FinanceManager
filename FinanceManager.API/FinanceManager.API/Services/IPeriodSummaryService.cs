using FinanceManager.API.Serialization.Types;
using FinanceManager.Types.Enums;
using System;

namespace FinanceManager.API.Services
{
    public interface IPeriodSummaryService
    {
        PeriodSummaryModel GetPeriodSummary(DateTime dateFromPeriod, int userId, PeriodUnit periodUnit = PeriodUnit.Month);
    }
}