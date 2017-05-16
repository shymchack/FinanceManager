using FinanceManager.API.Services;
using FinanceManager.DAL.Repositories;
using System.Web.Http;

namespace FinanceManager.API.Controllers
{
    public class AccountsController : BaseController
    {
        public IAccountsService AccountsService;

        public AccountsController(IAccountsService accountsService): base()
        {
            AccountsService = accountsService;
        }

        [HttpGet]
        public void CreateAccount(string name, int userID)
        {
            int id = AccountsService.CreateAccount(name, userID);
        }
    }
}
