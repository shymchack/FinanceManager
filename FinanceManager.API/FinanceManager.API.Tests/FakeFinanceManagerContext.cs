using Financemanager.Database.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinanceManager.Database.Entities;
using System.Data.Entity;

namespace FinanceManager.API.Tests
{
    public class FakeFinanceManagerContext : IFinanceManagerContext
    {

        public IDbSet<Account> Accounts { get; set; }

        public IDbSet<OperationSetting> OperationSettings { get; set; }

        public IDbSet<PeriodicExpense> PeriodicExpenses { get; set; }

        public IDbSet<PeriodicIncome> PeriodicIncomes { get; set; }

        public IDbSet<SingleExpense> SingleExpenses { get; set; }

        public IDbSet<SingleIncome> SingleIncomes { get; set; }

        public IDbSet<User> Users { get; set; }

        public IDbSet<UserAccount> UsersAccounts { get; set; }

        public FakeFinanceManagerContext()
        {
            Accounts = new FakeDbSet<Account>();
            OperationSettings = new FakeDbSet<OperationSetting>();
            PeriodicExpenses = new FakeDbSet<PeriodicExpense>();
            PeriodicIncomes = new FakeDbSet<PeriodicIncome>();
            SingleExpenses = new FakeDbSet<SingleExpense>();
            SingleIncomes = new FakeDbSet<SingleIncome>();
            Users = new FakeDbSet<User>();
            UsersAccounts = new FakeDbSet<UserAccount>();
        }

        public void Dispose()
        { }

        public void SaveChanges()
        { }
    }
}
