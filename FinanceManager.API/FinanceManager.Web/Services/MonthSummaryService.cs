using FinanceManager.BL.UserInput;
using FinanceManager.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinanceManager.Web.Services
{
    public class MonthSummaryService
    {
        public PeriodSummaryViewModel GetPeriodSummaryViewModel(IEnumerable<MoneyOperationStatusViewModel> moneyOperations)
        {
            PeriodSummaryViewModel model = new PeriodSummaryViewModel();

            model = new PeriodSummaryViewModel();
            model.PeriodTitle = "October 2018";

            model.CurrentPeriodExpensesAmount = 10133;
            model.PeriodBeginningPeriodExpensesAmount = 8956;

            model.CurrentPeriodIncomesAmount = 8015;
            model.PeriodBeginningPeriodIncomesAmount = 8015;

            model.CurrentTotalBalance = -20022;
            model.PeriodBeginningTotalBalance = -21471 + 8015 - 10133;

            List<MonthOperationViewModel> monthOperations = new List<MonthOperationViewModel>();

            foreach (MoneyOperationStatusViewModel moneyOperation in moneyOperations)
            {
                MonthOperationViewModel op = new MonthOperationViewModel();
                op.TotalAmount = 1600;
                op.AlreadyPayedAmount = moneyOperation.AlreadyPayedAmount;
                op.FinishDate = moneyOperation.FinishDate;
                op.Name = moneyOperation.Name;

                monthOperations.Add(op);
            }

            model.OperationsModel = new MonthOperationsTableViewModel();
            model.OperationsModel.MonthOperations = monthOperations;

            model.OperationsModel.AlreadyPayedLabel = "Already payed";
            model.OperationsModel.FinishDateLabel = "Finish date";
            model.OperationsModel.NameLabel = "Name";
            model.OperationsModel.PaymentLeftLabel = "Payment left";
            model.OperationsModel.TotalAmonutLabel = "Total Amount";

            model.NewMoneyOperation = new MoneyOperationViewData();
            model.NewMoneyOperation.AccountID = 3;

            return model;
        }
    }
}