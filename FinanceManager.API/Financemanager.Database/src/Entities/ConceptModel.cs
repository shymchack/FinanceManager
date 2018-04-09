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
        public MoneyOperation()
        {
            MoneyOperationChanges = new List<MoneyOperationChange>();
        }

        public int ID { get; set; }
        public int OperationSettingID { get; set; }
        public int? AccountID { get; set; }
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
        public virtual MoneyOperationSetting OperationSetting { get; set; }
        public virtual List<MoneyOperationChange> MoneyOperationChanges { get; set; }
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
            MoneyOperations = new List<MoneyOperation>();
        }

        public int ID { get; set; }
        public PeriodUnit ReservePeriodUnit { get; set; }
        public int ReservePeriodQuantity { get; set; }
        
        public virtual List<MoneyOperation> MoneyOperations { get; set; }
    }

    public class User
    {
        public User()
        {
            UsersAccounts = new List<UserAccount>();
            UsersTokens = new List<UserToken>();
        }

        public int ID { get; set; }
        public bool IsActive { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PassHash { get; set; }

        public virtual List<UserAccount> UsersAccounts { get; set; }
        public virtual List<UserToken> UsersTokens { get; set; }
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

    public class MoneyOperationChange
    {
        public int ID { get; set; }
        public DateTime ChangeDate { get; set; }
        public int MoneyOperationID { get; set; }
        public decimal ChangeAmount { get; set; }

        public virtual MoneyOperation MoneyOperation { get; set; }
    }

    public class Token
    {
        public Token()
        {
            UsersTokens = new List<UserToken>();
        }

        public int ID { get; set; }
        public string TokenData { get; set; }
        public DateTime ExpirationDate { get; set; }
        public List<UserToken> UsersTokens { get; set; }
    }

    public class UserToken
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int TokenID { get; set; }

        public virtual User User { get; set; }
        public virtual Token Token { get; set; }
    }
}