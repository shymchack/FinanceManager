using Financemanager.Database.Context;
using FinanceManager.DAL.Dtos;
using FinanceManager.DAL.Repositories.Contracts;
using System;
using System.Collections.Generic;

namespace FinanceManager.DAL
{
    public interface IUserAccountsUnitOfWork : IDisposable
    {
        FinanceManagerContext Context { get; }
        IUsersRepository UsersRepository { get; }
        IAccountsRepository AccountsRepository { get; }

        int CreateUser(string userName, string firstName, string lastName);
        bool DoesUserExist(string userName);
        List<AccountDto> GetAccounts();
        int CreateAccount(string name, int userID);

    }
}