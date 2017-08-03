using Financemanager.Database.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinanceManager.Database.Entities;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;

namespace FinanceManager.API.Tests
{
    public class FakeFinanceManagerContext : IFinanceManagerContext
    {

        public IDbSet<Account> Accounts { get; set; }

        public IDbSet<MoneyOperationSetting> MoneyOperationSettings { get; set; }

        public IDbSet<MoneyOperation> MoneyOperations { get; set; }
        
        public IDbSet<User> Users { get; set; }

        public IDbSet<UserAccount> UsersAccounts { get; set; }

        public ObjectContext ObjectContext
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public FakeFinanceManagerContext()
        {
            Accounts = new FakeDbSet<Account>();
            MoneyOperationSettings = new FakeDbSet<MoneyOperationSetting>();
            MoneyOperations = new FakeDbSet<MoneyOperation>();
            Users = new FakeDbSet<User>();
            UsersAccounts = new FakeDbSet<UserAccount>();
        }

        public void Dispose()
        { }

        public void SaveChanges()
        { }

        public DbEntityEntry Entry(object entity)
        {
            throw new NotImplementedException();
        }
    }
}
