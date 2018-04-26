using FinanceManager.API.Serialization.Types;
using FinanceManager.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

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
            return _periodSummaryService.GetPeriodSummary(DateTime.UtcNow, userId); //TODO: Should use server date
        }
    }
}
