using FinanceManager.Database.Context;
using FinanceManager.DAL.Dtos;
using FinanceManager.DAL.Repositories.Contracts;
using System;
using System.Collections.Generic;

namespace FinanceManager.DAL
{
    public interface IUserAccountsUnitOfWork : IDisposable
    {
        int CreateUser(string userName, string firstName, string lastName);
        bool DoesUserExist(string userName);
        List<AccountDto> GetAccounts();
        int CreateAccount(string name, int userID);

    }
}