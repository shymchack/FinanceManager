using FinanceManager.Database.Entities;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace FinanceManager.Database.Context
{
    public interface IFinanceManagerContext : IObjectContextAdapter, IDisposable
    {

        IDbSet<Account> Accounts { get; set; }

        IDbSet<MoneyOperationSetting> MoneyOperationSettings { get; set; }

        IDbSet<MoneyOperation> MoneyOperations { get; set; }

        IDbSet<User> Users { get; set; }

        IDbSet<UserAccount> UsersAccounts { get; set; }

        void SaveChanges();

        DbEntityEntry Entry(object entity);
    }
}