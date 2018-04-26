using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinanceManager.DAL.Dtos;
using FinanceManager.DAL.Repositories.Contracts;
using FinanceManager.Database.Context;
using FinanceManager.Database.Entities;

namespace FinanceManager.DAL.UnitOfWork
{
    public class MoneyOperationsUnitOfWork : IMoneyOperationsUnitOfWork
    {
        private IMoneyOperationsRepository _moneyOperationsRepository;
        private IAccountsRepository _accountsRepository;

        private IFinanceManagerContext _context;


        public MoneyOperationsUnitOfWork(IFinanceManagerContext context, IMoneyOperationsRepository moneyOperationRepository, IAccountsRepository accountsRepository)
        {
            _context = context;
            _moneyOperationsRepository = moneyOperationRepository;
            _accountsRepository = accountsRepository;
        }

        public int AddMoneyOperation(MoneyOperationDto moneyOperationDto)
        {
            MoneyOperation newMoneyOperation = null;
            Account account = _accountsRepository.GetAccountByID(moneyOperationDto.AccountID);
            if (account != null)
            {
                newMoneyOperation = _moneyOperationsRepository.CreateMoneyOperation(account);
            }
            if (newMoneyOperation != null)
            {
                ReadMoneyOperationDataFromDto(moneyOperationDto, newMoneyOperation);
                _moneyOperationsRepository.AddMoneyOperation(newMoneyOperation);
            }

            return newMoneyOperation != null ? newMoneyOperation.ID : -1; 
        }

        public MoneyOperationDto GetMoneyOperationById(int id)
        {
            MoneyOperation moneyOperation = _moneyOperationsRepository.GetMoneyOperationById(id);
            MoneyOperationDto moneyOperationDto = ReadMoneyOperationDtoFromData(moneyOperation);
            return moneyOperationDto;
        }


        //unify logic reading data backward and forward, extract the logic to separate classes
        private MoneyOperationDto ReadMoneyOperationDtoFromData(MoneyOperation moneyOperation)
        {
            //TODO: automapper
            var targetMoneyOperationDto = new MoneyOperationDto();
            targetMoneyOperationDto.Description = moneyOperation.Description;
            targetMoneyOperationDto.InitialAmount = moneyOperation.InitialAmount;
            targetMoneyOperationDto.IsActive = moneyOperation.IsActive;
            targetMoneyOperationDto.IsReal = moneyOperation.IsReal;
            targetMoneyOperationDto.Name = moneyOperation.Name;
            targetMoneyOperationDto.NextOperationExecutionDate = moneyOperation.NextOperationExecutionDate;
            targetMoneyOperationDto.OperationSettingID = moneyOperation.OperationSettingID;
            targetMoneyOperationDto.RepetitionUnit = moneyOperation.RepetitionUnit;
            targetMoneyOperationDto.RepetitionUnitQuantity = moneyOperation.RepetitionUnitQuantity;
            targetMoneyOperationDto.ValidityBeginDate = moneyOperation.ValidityBeginDate;
            targetMoneyOperationDto.ValidityEndDate = moneyOperation.ValidityEndDate;
            targetMoneyOperationDto.MoneyOperationChanges = moneyOperation.MoneyOperationChanges.Select(ch => new MoneyOperationChangeDto(){ ChangeAmount = ch.ChangeAmount, ChangeDate = ch.ChangeDate} ).ToList();

            return targetMoneyOperationDto;
        }

        private void ReadMoneyOperationDataFromDto(MoneyOperationDto sourceMoneyOperationDto, MoneyOperation targetMoneyOperation)
        {
            //TODO automapper
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

        public IEnumerable<MoneyOperationDto> GetMoneyOperationsByAccountsIDs(IEnumerable<int> accountsIds, DateTime date)
        {
            IEnumerable<MoneyOperation> moneyOperations = _moneyOperationsRepository.GetMoneyOperationsByAccountsIDs(accountsIds, date);
            List<MoneyOperationDto> moneyOperationsDtos = new List<MoneyOperationDto>();
            foreach (MoneyOperation moneyOperation in moneyOperations)
            {
                MoneyOperationDto dto = ReadMoneyOperationDtoFromData(moneyOperation);
                moneyOperationsDtos.Add(dto);
            }

            return moneyOperationsDtos;
        }
    }
}
