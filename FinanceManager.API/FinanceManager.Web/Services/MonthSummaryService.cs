using FinanceManager.BL.UserInput;
using FinanceManager.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FinanceManager.Web.Controllers;

namespace FinanceManager.Web.Services
{
    public class MonthSummaryService
    {
        //TODO move this logic to API.
        public PeriodSummaryViewModel GetPeriodSummaryViewModel(
            IEnumerable<MoneyOperationStatusViewModel> moneyOperations, 
            IEnumerable<AccountViewModel> accounts)
        {
            PeriodSummaryViewModel model = new PeriodSummaryViewModel();

            model = new PeriodSummaryViewModel();
            model.PeriodTitle = "October 2018";


            List<MonthOperationViewModel> monthOperations = new List<MonthOperationViewModel>();

            foreach (MoneyOperationStatusViewModel moneyOperation in moneyOperations)
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
    }
}