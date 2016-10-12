using FinanceManager.DAL.Dtos;
using FinanceManager.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace FinanceManager.API.Controllers
{
    public class AccountsController : ApiController
    {
        public IAccountsRepository AccountsRepository { get; set; }

        public IEnumerable<AccountDto> GetAccounts()
        {
            throw new NotImplementedException();
        }
    }
}
