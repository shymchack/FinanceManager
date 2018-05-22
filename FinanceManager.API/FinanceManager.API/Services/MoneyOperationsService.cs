using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FinanceManager.DAL.UnitOfWork;
using FinanceManager.DAL.Dtos;
using FinanceManager.BL.UserInput;
using FinanceManager.API.Serialization;
using FinanceManager.BL;
using FinanceManager.API.Serialization.Types;
using FinanceManager.Types.Enums;
using FinanceManager.DAL;

namespace FinanceManager.API.Services
{
    public class MoneyOperationsService : IMoneyOperationsService
    {
        IMoneyOperationsUnitOfWork _moneyOperationUOW;
        IUserAccountsUnitOfWork _userAccountsUnitOfWork;
        IMoneyOperationLogic _moneyOperationLogic;

        public MoneyOperationsService(IMoneyOperationsUnitOfWork moneyOperationUOW, IMoneyOperationLogic moneyOperationLogic)
        {
            _moneyOperationUOW = moneyOperationUOW;
            _moneyOperationLogic = moneyOperationLogic;
        }

        public int AddMoneyOperation(MoneyOperationModel moneyOperation)
        {
            MoneyOperationDto moneyOperationDto = _moneyOperationLogic.ConvertUserInputToDto(moneyOperation);
            int newMoneyOperationId = _moneyOperationUOW.AddMoneyOperation(moneyOperationDto);

            //TODO pass response that would be more verbose than just an ID
            return newMoneyOperationId;
        }

        public MoneyOperationModel GetMoneyOperationById(int id)
        {
            MoneyOperationDto moneyOperationDto = _moneyOperationUOW.GetMoneyOperationById(id);
            MoneyOperationModel viewData = _moneyOperationLogic.ConvertDtoToViewData(moneyOperationDto);
            return viewData;
        }

        public IEnumerable<MoneyOperationStatus> GetMoneyOperationsByAccountsIds(IEnumerable<int> accountsIds, DateTime date)
        {
            IEnumerable<MoneyOperationDto> moneyOperationDtos = _moneyOperationUOW.GetMoneyOperationsByAccountsIDs(accountsIds, date);
            List<MoneyOperationStatus> moneyOperationStatuses = new List<MoneyOperationStatus>();

            foreach(MoneyOperationDto moneyOperationDto in moneyOperationDtos)
            {
                MoneyOperationStatus status = GetMoneyOperationStatusFromDto(moneyOperationDto, date);
                if (status != null)
                    moneyOperationStatuses.Add(status);
            }

            return moneyOperationStatuses;

        }

        public MoneyOperationScheduleModel GetMoneyOperationSchedule(int moneyOperationId)
        {
            MoneyOperationDto moneyOperation = _moneyOperationUOW.GetMoneyOperationById(moneyOperationId);
            
            MoneyOperationScheduleModel model = GetMoneyOperationScheduleFromDto(moneyOperation);
            return model;
        }


        private MoneyOperationStatus GetMoneyOperationStatusFromDto(MoneyOperationDto moneyOperationDto, DateTime date)
        {
            MoneyOperationStatus status = _moneyOperationLogic.PrepareMoneyOperationStatus(moneyOperationDto, date);

            return status;
        }

        private MoneyOperationScheduleModel GetMoneyOperationScheduleFromDto(MoneyOperationDto moneyOperationDto)
        {
            return _moneyOperationLogic.GetMoneyOperationSchedule(moneyOperationDto);
        }
    }
}