﻿using FinanceManager.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.DAL.Repositories.Contracts
{
    public interface IAccountsOperationsRepository
    {
        void CreateMoneyOperation(MoneyOperation moneyOperation);
    }
}
