using System;
using System.Collections.Generic;

namespace FinanceManager.Database.Entities
{
    public class Account
    {
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public decimal InitialAmount { get; set; } //TODO: Is it really necessary?
        public decimal CurrentAmount { get; set; }

        public virtual List<User> Users { get; set; }
    }

    public class SingleIncome
    {
        /// <summary>
        /// Determines whether the income should be processed or not.
        /// </summary>
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public IncomeCategory Category { get; set; }
        /// <summary>
        /// The amoutn of incoming money. Should be positive
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Determines whether the income has been already taken into account.
        /// </summary>
        public bool IsAlreadyProcessed { get; set; }
        /// <summary>
        /// Determines whether this is a real or hypothetic income.
        /// </summary>
        public bool IsReal { get; set; }

        public virtual Account Account { get; set; }
    }

    public class PeriodicIncome
    {
        public bool IsReal { get; set; }
        public bool IsActive { get; set; }
        public DateTime ValidityBeginDate { get; set; }
        public DateTime ValidityEndDate { get; set; }
        public short RepetitionUnitQuantity { get; set; } //TODO: Rename
        public PeriodUnit RepetitionUnit { get; set; } //TODO: Rename
        public decimal InitialAmount { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// Determines whether this is a real or hypothetic income.
        /// </summary>
        
        public virtual Account Account { get; set; }
    }

    public class IncomeCategory
    {
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public IncomeCategory SubCategory { get; set; }
    }

    public class User
    {
        public bool IsActive { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual List<Account> Account { get; set; }
    }

    public class UsersAccounts
    {
        public bool IsActive { get; set; }
        public int UserId { get; set; }
        public int AccountId { get; set; }

        public virtual Account Account { get; set; }
        public virtual User User { get; set; }
    }
}

namespace FinanceManager.Database.Entities
{
    public enum PeriodUnit
    {
        Second,
        Minute,
        Hour,
        Day,
        Week,
        Month,
        Year
    }
}