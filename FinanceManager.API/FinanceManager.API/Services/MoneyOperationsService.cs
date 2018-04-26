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

        public int AddMoneyOperation(MoneyOperationViewData moneyOperation)
        {
            MoneyOperationDto moneyOperationDto = _moneyOperationLogic.ConvertUserInputToDto(moneyOperation);
            int newMoneyOperationId = _moneyOperationUOW.AddMoneyOperation(moneyOperationDto);

            //TODO pass response that would be more verbose than just an ID
            return newMoneyOperationId;
        }

        public MoneyOperationViewData GetMoneyOperationById(int id)
        {
            MoneyOperationDto moneyOperationDto = _moneyOperationUOW.GetMoneyOperationById(id);
            MoneyOperationViewData viewData = _moneyOperationLogic.ConvertDtoToViewData(moneyOperationDto);
            return viewData;
        }

        public IEnumerable<MoneyOperationStatus> GetMoneyOperationsByAccountsIds(IEnumerable<int> accountsIds, DateTime date)
        {
            IEnumerable<MoneyOperationDto> moneyOperationDtos = _moneyOperationUOW.GetMoneyOperationsByAccountsIDs(accountsIds, date);
            List<MoneyOperationStatus> moneyOperationStatuses = new List<MoneyOperationStatus>();

            foreach(MoneyOperationDto moneyOperationDto in moneyOperationDtos)
            {
                MoneyOperationStatus status = GetMoneyOperationStatusFromDto(moneyOperationDto, date);
                moneyOperationStatuses.Add(status);
            }

            return moneyOperationStatuses;

        }

        public PeriodSummaryViewModel GetPeriodSummary(DateTime dateFromPeriod, int userId, PeriodUnit periodUnit = PeriodUnit.Month)
        {
            //TODO implement periodUnit
            IEnumerable<AccountDto> accounts = _userAccountsUnitOfWork.GetAccountsByUserId(userId);
            IEnumerable<MoneyOperationStatus> moneyOperations = GetMoneyOperationsByAccountsIds(accounts.Select(a => a.ID), dateFromPeriod);
            PeriodSummaryViewModel model = new PeriodSummaryViewModel();

            model = new PeriodSummaryViewModel();
            model.PeriodTitle = "October 2018";


            List<MonthOperationViewModel> monthOperations = new List<MonthOperationViewModel>();

            foreach (MoneyOperationStatus moneyOperation in moneyOperations)
            {
                MonthOperationViewModel op = new MonthOperationViewModel();
                op.TotalAmount = moneyOperation.InitialAmount;
                op.AlreadyPayedAmount = moneyOperation.AlreadyPayedAmount;
                op.CurrentPeriodPayedAmount = moneyOperation.CurrentPeriodPayedAmount;
                op.FinishDate = moneyOperation.FinishDate;
                op.BeginningDate = moneyOperation.BeginningDate;
                op.Name = moneyOperation.Name;
                monthOperations.Add(op);
            }

            model.CurrentPeriodExpensesAmount = 10133;
            model.PeriodBeginningPeriodExpensesAmount = 8956;

            model.CurrentPeriodIncomesAmount = 8015;
            model.PeriodBeginningPeriodIncomesAmount = 8015;

            model.CurrentTotalBalance = (double)accounts.Sum(a => a.CurrentAmount);
            model.PeriodBeginningTotalBalance = (double)accounts.Sum(a => a.CurrentAmount) + (double)monthOperations.Where(mo => mo.FinishDate >= DateTime.UtcNow && mo.BeginningDate <= DateTime.UtcNow).Sum(mo => mo.CurrentPeriodPayedAmount); //TODO: UtcNow date should be taken from server!
            model.NextPeriodBeginningTotalBalance = 20000;

            model.OperationsModel = new MonthOperationsTableViewModel();
            model.OperationsModel.MonthOperations = monthOperations;

            model.OperationsModel.AlreadyPayedLabel = "Already payed";
            model.OperationsModel.FinishDateLabel = "Finish date";
            model.OperationsModel.NameLabel = "Name";
            model.OperationsModel.PaymentLeftLabel = "Payment left";
            model.OperationsModel.TotalAmonutLabel = "Total Amount";
            model.OperationsModel.CurrentPeriodPayedLabel = "Current month payed";

            model.NewMoneyOperation = new MoneyOperationViewData();
            model.NewMoneyOperation.AccountID = 3;

            return model;
        }

        private MoneyOperationStatus GetMoneyOperationStatusFromDto(MoneyOperationDto moneyOperationDto, DateTime date)
        {
            MoneyOperationStatus status = _moneyOperationLogic.PrepareMoneyOperationStatus(moneyOperationDto, date);

            return status;
        }
    }
}