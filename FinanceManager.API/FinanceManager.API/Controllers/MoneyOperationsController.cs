using FinanceManager.API.Services;
using FinanceManager.BL;
using System;
using System.Web.Http;

namespace FinanceManager.API.Controllers
{
    public class MoneyOperationsController : BaseController
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

        public MoneyOperationScheduleModel GetMoneyOperationSchedule(int moneyOperationId)
        {
            return _moneyOperationsService.GetMoneyOperationSchedule(moneyOperationId);
        }
    }
}
