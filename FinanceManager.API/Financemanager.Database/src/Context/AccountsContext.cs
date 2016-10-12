using FinanceManager.Database.Entities;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Financemanager.Database.Context
{
    public class FinanceManagerContext : DbContext
    {
        public DbSet<Account> Accounts;
        public DbSet<User> Users { get; set; }
        public DbSet<SingleIncome> SingleIncomes { get; set; }
        public DbSet<PeriodicIncome> PeriodicIncomes { get; set; }
        public DbSet<UsersAccounts> UsersAccounts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
