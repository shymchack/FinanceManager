using FinanceManager.API.Services;
using System;
using System.Web.Http;

namespace FinanceManager.API.Controllers
{
    public class MoneyOperationsController : ApiController
    {
        IMoneyOperationsService _moneyOperationsService;

        public MoneyOperationsController(IMoneyOperationsService moneyOperationsService)
        {
            _moneyOperationsService = moneyOperationsService;
        }

        public IHttpActionResult GetMoneyOperationsByAccountId(int accountId)
        {
            throw new NotImplementedException();
        }
    }
}
