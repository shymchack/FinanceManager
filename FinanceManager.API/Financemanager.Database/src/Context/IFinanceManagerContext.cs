using FinanceManager.Database.Entities;
using System;
using System.Data.Entity;

namespace Financemanager.Database.Context
{
    public interface IFinanceManagerContext : IDisposable
    {
        IDbSet<Account> Accounts { get; set; }
        IDbSet<User> Users { get; set; }
        IDbSet<SingleIncome> SingleIncomes { get; set; }
        IDbSet<PeriodicIncome> PeriodicIncomes { get; set; }
        IDbSet<SingleExpense> SingleExpenses { get; set; }
        IDbSet<PeriodicExpense> PeriodicExpenses { get; set; }
        IDbSet<OperationSetting> OperationSettings { get; set; }
        IDbSet<UserAccount> UsersAccounts { get; set; }

        void SaveChanges();
    }
}