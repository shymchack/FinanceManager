using FinanceManager.Web.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Web.Mvc;

namespace FinanceManager.Web.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController()
        {

        }

        public ActionResult Index()
        {
            PeriodSummaryViewModel periodSummary = null;

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://localhost:35816/api/PeriodSummary/");
                var response = httpClient.GetAsync($"GetPeriodSummary?userId={1}");
                response.Wait();
                string result = response.Result.Content.ReadAsStringAsync().Result;
                periodSummary = JsonConvert.DeserializeObject<PeriodSummaryViewModel>(result);
            }

            return View(periodSummary ?? new PeriodSummaryViewModel());
        }

        public ActionResult Details(int moneyOperationId)
        {
            MoneyOperationScheduleViewModel schedule = null;

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://localhost:35816/api/MoneyOperations/");
                var response = httpClient.GetAsync($"GetMoneyOperationSchedule?moneyOperationId={moneyOperationId}");
                response.Wait();
                string result = response.Result.Content.ReadAsStringAsync().Result;
                schedule = JsonConvert.DeserializeObject<MoneyOperationScheduleViewModel>(result);
            }
            return View(schedule ?? new MoneyOperationScheduleViewModel());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}