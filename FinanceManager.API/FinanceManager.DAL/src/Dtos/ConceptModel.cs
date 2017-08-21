using FinanceManager.Types.Enums;
using System;
using System.Collections.Generic;

namespace FinanceManager.DAL.Dtos
{
    public class AccountDto
    {
        public decimal CurrentAmount { get; set; }
        public decimal InitialAmount { get; set; } //TODO: Is it really necessary?
        public string Name { get; set; }

        public virtual List<UserDto> Users { get; set; }
    }

    public class MoneyOperationDto
    {
        public int AccountID { get; set; }
        /// <summary>
        /// Determines whether the income should be processed or not.
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Determines whether the income has been already taken into account.
        /// </summary>
        public bool IsAlreadyProcessed { get; set; }
        /// <summary>
        /// Determines whether this is a real or hypothetic operation.
        /// </summary>
        public bool IsReal { get; set; }
        public DateTime ValidityBeginDate { get; set; }
        public DateTime ValidityEndDate { get; set; }
        public DateTime NextOperationExecutionDate { get; set; }
        public short RepetitionUnitQuantity { get; set; } //TODO: Rename
        public PeriodUnit RepetitionUnit { get; set; } //TODO: Rename
        public decimal InitialAmount { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public int OperationSettingID { get; set; }
        public DateTime LastOrFirstOperationExecutionDate
        {
            get
            {
                DateTime lastOrFirstOperationExecutionDate = NextOperationExecutionDate;
                if (lastOrFirstOperationExecutionDate == default(DateTime))
                {
                    lastOrFirstOperationExecutionDate = ValidityBeginDate;
                }
                return lastOrFirstOperationExecutionDate;
            }
        }
    }

    public class UserDto
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual List<AccountDto> Account { get; set; }
    }

    public class UsersAccountsDto
    {
        public int UserId { get; set; }
        public int AccountId { get; set; }

        public virtual AccountDto Account { get; set; }
        public virtual UserDto User { get; set; }
    }
}