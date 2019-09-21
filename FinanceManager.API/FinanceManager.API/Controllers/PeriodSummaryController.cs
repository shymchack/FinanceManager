using FinanceManager.API.Serialization.Types;
using FinanceManager.API.Services;
using System;

namespace FinanceManager.API.Controllers
{
    public class PeriodSummaryController : BaseController
    {
        private IPeriodSummaryService _periodSummaryService;

        public PeriodSummaryController(IPeriodSummaryService periodSummaryService)
        {
            _periodSummaryService = periodSummaryService;
        }

        public PeriodSummaryModel GetPeriodSummary(int userId)
        {
            var periodSummary = _periodSummaryService.GetPeriodSummary(new DateTime(2018, 11, 10), userId); //TODO: Should use server date
            return periodSummary;
        }
    }
}
