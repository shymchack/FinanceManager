using FinanceManager.API.Services;
using FinanceManager.BL;
using FinanceManager.BL.UserInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            var moneyOperations = _moneyOperationsService.GetMoneyOperationsByAccountID(accountId, DateTime.UtcNow);

            return Ok(moneyOperations ?? new List<MoneyOperationStatus>());
        }
    }
}
