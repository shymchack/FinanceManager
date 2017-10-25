﻿using FinanceManager.DAL.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinanceManager.Database.Entities;
using FinanceManager.Database.Context;
using System.Data.Entity;

namespace FinanceManager.DAL.Repositories
{
    public class MoneyOperationsRepository : FinanceManagerRepository, IMoneyOperationsRepository
    {
        public MoneyOperationsRepository(IFinanceManagerContext context) : base(context)
        {
        }

        public MoneyOperation CreateMoneyOperation(Account account)
        {
            MoneyOperation moneyOperation = Context.MoneyOperations.Create();
            moneyOperation.Account = account;
            Context.MoneyOperations.Add(moneyOperation);
            return moneyOperation;
        }

        public void AddMoneyOperation(MoneyOperation moneyOperation)
        {
            if (Context.Entry(moneyOperation).State == EntityState.Added)
            {
                Context.SaveChanges();
            }
        }

        public MoneyOperation GetMoneyOperationById(int id)
        {
            return Context.MoneyOperations.FirstOrDefault(mo => mo.ID == id);
        }
    }
}