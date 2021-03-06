﻿using FinanceManager.DAL.Dtos;
using System.Collections.Generic;
using FinanceManager.Database.Context;
using System;
using FinanceManager.Database.Entities;

namespace FinanceManager.DAL.Repositories.Contracts
{
    public interface IAccountsRepository : IDisposable
    {
        List<AccountDto> GetAccounts();
        int AddAccount(Account account);
        Account CreateAccount();
        Account GetAccountByID(int accountID);
        IEnumerable<Account> GetAccountsByUserId(int userId);
    }
}
