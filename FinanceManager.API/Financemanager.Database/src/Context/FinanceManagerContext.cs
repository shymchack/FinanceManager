using FinanceManager.Database.Entities;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace Financemanager.Database.Context
{
    public class FinanceManagerContext : DbContext
    {
        public IDbSet<Account> Accounts { get; set; }
        public IDbSet<User> Users { get; set; }
        public IDbSet<SingleIncome> SingleIncomes { get; set; }
        public IDbSet<PeriodicIncome> PeriodicIncomes { get; set; }
        public IDbSet<SingleExpense> SingleExpenses { get; set; }
        public IDbSet<PeriodicExpense> PeriodicExpenses { get; set; }
        public IDbSet<OperationSetting> OperationSettings { get; set; }
        public IDbSet<UserAccount> UsersAccounts { get; set; }

        public FinanceManagerContext() : base("name=FinanceManagerConnectionString")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
