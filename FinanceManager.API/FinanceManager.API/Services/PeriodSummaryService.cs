using FinanceManager.API.Serialization.Types;
using FinanceManager.BL;
using FinanceManager.BL.UserInput;
using FinanceManager.DAL;
using FinanceManager.DAL.Dtos;
using FinanceManager.DAL.UnitOfWork;
using FinanceManager.Types.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinanceManager.API.Services
{
    public class PeriodSummaryService : IPeriodSummaryService
    {
        IMoneyOperationsService _moneyOperationsService;
        IUserAccountsUnitOfWork _userAccountsUnitOfWork;

        //TODO maybe should use only Services, no UOWs
        public PeriodSummaryService(IMoneyOperationsService moneyOperationsService, IUserAccountsUnitOfWork userAccountsUnitOfWork)
        {
            _moneyOperationsService = moneyOperationsService;
            _userAccountsUnitOfWork = userAccountsUnitOfWork;
        }

        public PeriodSummaryModel GetPeriodSummary(DateTime dateFromPeriod, int userId, PeriodUnit periodUnit = PeriodUnit.Month)
        {
            IEnumerable<AccountDto> accounts = _userAccountsUnitOfWork.GetAccountsByUserId(userId);
            IEnumerable<MoneyOperationStatusModel> moneyOperations = _moneyOperationsService.GetMoneyOperationsByAccountsIds(accounts.Select(a => a.ID), dateFromPeriod);

            //TODO implement periodUnit
            PeriodSummaryModel model = new PeriodSummaryModel();

            model = new PeriodSummaryModel();
            model.PeriodTitle = "October 2018";
            
            model.CurrentPeriodIncomesAmount = 8015; //TODO needed to add mmoney operation modification date
            model.PeriodBeginningPeriodIncomesAmount = (double)moneyOperations.Sum(mo => mo.CurrentPeriodIncomes);

            model.CurrentTotalBalance = (double)accounts.Sum(a => a.CurrentAmount);

            model.OperationsModel = new PeriodOperationsModel();
            model.OperationsModel.PeriodOperations = moneyOperations;

            model.OperationsModel.AlreadyPayedLabel = "Already payed";
            model.OperationsModel.FinishDateLabel = "Finish date";
            model.OperationsModel.NameLabel = "Name";
            model.OperationsModel.PaymentLeftLabel = "Payment left";
            model.OperationsModel.TotalAmonutLabel = "Total Amount";
            model.OperationsModel.CurrentPeriodEndAmountLabel = "Current period end amount";
            model.OperationsModel.CurrentPeriodPayedAmountLabel= "Current period payed amount";
            model.OperationsModel.CurrentPeriodBudgetedAmountLabel = "Current period budgeted amount";
            model.OperationsModel.CurrentPeriodPaymentLeftLabel= "Current period payment left";

            model.NewMoneyOperation = new MoneyOperationModel();
            model.NewMoneyOperation.AccountID = 3;

            return model;
        }
    }
}