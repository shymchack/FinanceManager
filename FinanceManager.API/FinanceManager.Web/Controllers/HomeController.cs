using FinanceManager.BL.UserInput;
using FinanceManager.Web.Models;
using FinanceManager.Web.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
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
            IEnumerable<MoneyOperationStatusViewModel> moneyOperations = null;

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://localhost:35816/api/MoneyOperations/");
                var response = httpClient.GetAsync($"GetMoneyOperationsByAccountId?accountId={1}");
                response.Wait();
                string result = response.Result.Content.ReadAsStringAsync().Result;
                moneyOperations = JsonConvert.DeserializeObject<IEnumerable<MoneyOperationStatusViewModel>>(result);
            }

            var model = new MonthSummaryService().GetMonthSummaryViewModel(moneyOperations);

            return View(model);
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