using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinanceManager.DAL.Dtos;
using FinanceManager.DAL.Repositories.Contracts;
using Financemanager.Database.Context;
using FinanceManager.Database.Entities;

namespace FinanceManager.DAL.src.UnitOfWork
{
    public class MoneyOperationUnitOfWork : IMoneyOperationsUnitOfWork
    {
        private IMoneyOperationsRepository _moneyOperationsRepository;
        private IAccountsRepository _accountsRepository;

        private IFinanceManagerContext _context;


        public MoneyOperationUnitOfWork(IFinanceManagerContext context, IMoneyOperationsRepository moneyOperationRepository, IAccountsRepository accountsRepository)
        {
            _context = context;
            _moneyOperationsRepository = moneyOperationRepository;
            _accountsRepository = accountsRepository;
        }

        public void AddMoneyOperation(MoneyOperationDto moneyOperationDto)
        {
            MoneyOperation newMoneyOperation = null;
            Account account = _accountsRepository.GetAccountByID(moneyOperationDto.AccountID);
            if (account != null)
            {
                newMoneyOperation = _moneyOperationsRepository.CreateMoneyOperation(account);
            }
            if (newMoneyOperation != null)
            {
                ReaMoneyOperationDataFromDto(moneyOperationDto, newMoneyOperation);
                _moneyOperationsRepository.AddMoneyOperation(newMoneyOperation);
            }
        }

        private void ReaMoneyOperationDataFromDto(MoneyOperationDto sourceMoneyOperationDto, MoneyOperation targetMoneyOperation)
        {
            targetMoneyOperation.Description = sourceMoneyOperationDto.Description;
            targetMoneyOperation.InitialAmount = sourceMoneyOperationDto.InitialAmount;
            targetMoneyOperation.IsActive = sourceMoneyOperationDto.IsActive;
            targetMoneyOperation.IsReal = sourceMoneyOperationDto.IsReal;
            targetMoneyOperation.Name = sourceMoneyOperationDto.Name;
            targetMoneyOperation.NextOperationExecutionDate = sourceMoneyOperationDto.NextOperationExecutionDate;
            targetMoneyOperation.OperationSettingID = sourceMoneyOperationDto.OperationSettingID;
            targetMoneyOperation.RepetitionUnit = sourceMoneyOperationDto.RepetitionUnit;
            targetMoneyOperation.RepetitionUnitQuantity = sourceMoneyOperationDto.RepetitionUnitQuantity;
            targetMoneyOperation.ValidityBeginDate = sourceMoneyOperationDto.ValidityBeginDate;
            targetMoneyOperation.ValidityEndDate = sourceMoneyOperationDto.ValidityEndDate;
        }
    }
}
