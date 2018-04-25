using FinanceManager.API.Services;
using FinanceManager.DAL.Dtos;
using System.Collections.Generic;
using System.Web.Http;

namespace FinanceManager.API.Controllers
{
    public class AccountsController : BaseController
    {
        IAccountsService _accountsService;

        public AccountsController(IAccountsService accountsService): base()
        {
            _accountsService = accountsService;
        }

        [HttpGet]
        public void CreateAccount(string name, int userID)
        {
            int id = _accountsService.CreateAccount(name, userID);
        }

        [HttpGet]
        public IHttpActionResult GetAccountsByUserId(int userId)
        {
            IEnumerable<AccountDto> accounts = _accountsService.GetAccountsByUserId(userId);
            return Ok(accounts ?? new List<AccountDto>());
        }
    }
}
