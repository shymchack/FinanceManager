using FinanceManager.BL.UserInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinanceManager.Web.Models
{
    public class MonthSummaryViewModel
    {
        public string MonthName { get; set; }

        public double CurrentTotalBalance { get; set; }
        public double MonthBeginningTotalBalance { get; set; }
        public double TotalBalanceDifference { get { return CurrentTotalBalance - MonthBeginningTotalBalance; } }


        public double CurrentMonthBalance { get { return CurrentMonthIncomesAmount - CurrentMonthExpensesAmount; } }
        public double MonthBeginningMonthBalance { get { return MonthBeginningMonthIncomesAmount - MonthBeginningMonthExpensesAmount; } }
        public double MonthBalanceDifference { get { return CurrentMonthBalance - MonthBeginningMonthBalance; } }


        public double MonthBeginningMonthExpensesAmount { get; set; }
        public double CurrentMonthExpensesAmount { get; set; }
        public double MonthExpensesDifference { get { return CurrentMonthExpensesAmount - MonthBeginningMonthExpensesAmount; } }

        public double MonthBeginningMonthIncomesAmount { get; set; }
        public double CurrentMonthIncomesAmount { get; set; }
        public double MonthIncomesDifference { get { return CurrentMonthIncomesAmount - MonthBeginningMonthIncomesAmount; } }

        public MonthOperationsTableViewModel OperationsModel { get; set; }

        public MoneyOperationViewData NewMoneyOperation { get; set; }
    }
}