using System;
using System.Collections.Generic;

namespace FinanceManager.Database.Entities
{
    public class Account
    {
        public decimal CurrentAmount { get; set; }
        public decimal InitialAmount { get; set; } //TODO: Is it really necessary?

        public virtual List<User> Users { get; set; }
    }

    public class SingleIncome
    {
        public DateTime IncomeDate { get; set; }
        /// <summary>
        /// Determines whether the income should be processed or not.
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Determines whether the income has been already taken into account.
        /// </summary>
        public bool IsAlreadyProcessed { get; set; }
        /// <summary>
        /// The amoutn of incoming money. Should be positive
        /// </summary>
        public decimal IncomeAmount { get; set; }
        /// <summary>
        /// Determines whether this is a real or hypothetic income.
        /// </summary>
        public bool IsReal { get; set; }

        public virtual Account Account { get; set; }
    }

    public class PeriodicIncome
    {
        public DateTime ValidityBeginDate { get; set; }
        public DateTime ValidityEndDate { get; set; }
        public short RepetitionUnitQuantity { get; set; } //TODO: Rename
        public PeriodUnit RepetitionUnit { get; set; } //TODO: Rename
        public decimal InitialAmount { get; set; }
        /// <summary>
        /// Determines whether this is a real or hypothetic income.
        /// </summary>
        public bool IsReal { get; set; }
        public bool IsActive { get; set; }


        public virtual Account Account { get; set; }
    }

    public class User
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual List<Account> Account { get; set; }
    }

    public class UsersAccounts
    {
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