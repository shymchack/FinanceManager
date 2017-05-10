using FinanceManager.API.Services;
using FinanceManager.DAL.Repositories;
using System.Web.Http;

namespace FinanceManager.API.Controllers
{
    public class AccountsController : ApiController
    {
        public IAccountsService AccountsService;

        public AccountsController(IAccountsService accountsService)
        {
            AccountsService = accountsService;
        }

        [HttpGet]
        public void CreateAccount(string name)
        {
            int id = AccountsService.CreateAccount(name);
        }
    }
}
