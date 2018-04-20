using FinanceManager.BD.UserInput;
using FinanceManager.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinanceManager.Web.Services
{
    public class MonthSummaryService
    {
        public MonthSummaryViewModel GetMonthSummaryViewModel(IEnumerable<MoneyOperationStatusViewModel> moneyOperations)
        {
            MonthSummaryViewModel model = new MonthSummaryViewModel();

            model = new MonthSummaryViewModel();
            model.MonthName = "October 2018";

            model.CurrentMonthExpensesAmount = 10133;
            model.MonthBeginningMonthExpensesAmount = 8956;

            model.CurrentMonthIncomesAmount = 8015;
            model.MonthBeginningMonthIncomesAmount = 8015;

            model.CurrentTotalBalance = -20022;
            model.MonthBeginningTotalBalance = -21471 + 8015 - 10133;

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