using FinanceManager.Database.Entities;
using System;
using System.Data.Entity;

namespace Financemanager.Database.Context
{
    public interface IFinanceManagerContext : IDisposable
    {

        IDbSet<Account> Accounts { get; set; }

        IDbSet<MoneyOperationSetting> MoneyOperationSettings { get; set; }

        IDbSet<MoneyOperation> MoneyOperations { get; set; }

        IDbSet<User> Users { get; set; }

        IDbSet<UserAccount> UsersAccounts { get; set; }

        void SaveChanges();
    }
}