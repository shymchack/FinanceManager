using FinanceManager.Types.Enums;
using System;
using System.Collections.Generic;

namespace FinanceManager.Database.Entities
{
    public class Account
    {
        public int ID { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal InitialAmount { get; set; } //TODO: Is it really necessary?
        public decimal CurrentAmount { get; set; }
        public DateTime CreationDate { get; set; }
        
        public virtual List<UserAccount> UsersAccounts { get; set; }
    }

    public class MoneyOperation
    {
        public int ID { get; set; }
        public int OperationSettingID { get; set; }
        /// <summary>
        /// Determines whether this is a real or hypothetic operation.
        /// </summary>
        public bool IsReal { get; set; }
        public bool IsActive { get; set; }
        public DateTime ValidityBeginDate { get; set; }
        public DateTime ValidityEndDate { get; set; }
        public DateTime NextOperationExecutionDate { get; set; }
        /// <summary>
        /// If 0, no repetition occurs.
        /// </summary>
        public short RepetitionUnitQuantity { get; set; } //TODO: Rename
        public PeriodUnit RepetitionUnit { get; set; } //TODO: Rename
        public decimal InitialAmount { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        public virtual Account Account { get; set; }
    }

    public class MoneyOperationCategory
    {
        public int ID { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public MoneyOperationCategory SubCategory { get; set; }
    }

    public class MoneyOperationSetting
    {
        public MoneyOperationSetting()
        {
            MoneyOperation = new List<MoneyOperation>();
        }

        public int ID { get; set; }
        public PeriodUnit ReservePeriodUnit { get; set; }
        public int ReservePeriodQuantity { get; set; }
        
        public virtual List<MoneyOperation> MoneyOperation { get; set; }
    }

    public class User
    {
        public int ID { get; set; }
        public bool IsActive { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual List<UserAccount> UsersAccounts { get; set; }
    }

    public class UserAccount
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int AccountID { get; set; }
        public bool IsActive { get; set; }

        public virtual Account Account { get; set; }
        public virtual User User { get; set; }
    }
}