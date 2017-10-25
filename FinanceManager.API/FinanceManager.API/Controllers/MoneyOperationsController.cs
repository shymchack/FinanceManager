using FinanceManager.BD.UserInput;
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
        public MoneyOperationViewData GetMoneyOperationsByAccountId(int accountId)
        {


            return new MoneyOperationViewData();
        }
    }
}
