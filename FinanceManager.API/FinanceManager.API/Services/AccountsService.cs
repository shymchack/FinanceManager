using FinanceManager.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinanceManager.API.Services
{
    public class AccountsService : IAccountsService
    {
        IAccountsRepository AccountsRepository { get; set; }

        public AccountsService()
        {

        }

        public AccountsService(IAccountsRepository accountsRepository)
        {
            AccountsRepository = accountsRepository;
        }

        public int CreateAccount(string name, int userID)
        {
            return AccountsRepository.CreateAccount(name, userID);
        }
    }
}