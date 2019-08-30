using FinanceManager.API.Services;
using FinanceManager.BL;
using FinanceManager.BL.UserInput;
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
            //TODO should i pass date here?
            return _moneyOperationsService.GetMoneyOperationSchedule(moneyOperationId, DateTime.UtcNow);
        }

        public void AddMoneyOperation(MoneyOperationModel moneyOperation)
        {
            _moneyOperationsService.AddMoneyOperation(moneyOperation);
        }

        public void AddMoneyOperationChange(MoneyOperationChangeModel moneyOperationChange)
        {
            _moneyOperationsService.AddMoneyOperationChange(moneyOperationChange);
        }
    }
}
