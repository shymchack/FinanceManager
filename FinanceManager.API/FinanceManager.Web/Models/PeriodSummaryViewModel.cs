using FinanceManager.BL.UserInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinanceManager.Web.Models
{
    public class PeriodSummaryViewModel
    {
        public PeriodSummaryViewModel()
        {
            OperationsModel = new PeriodOperationsViewModel();
            NewMoneyOperation = new MoneyOperationModel();
        }
        public string PeriodTitle { get; set; }

        public double CurrentTotalBalance { get; set; }
        public double PeriodBeginningTotalBalance { get; set; }
        public double TotalBalanceDifference { get; set; }


        public double CurrentPeriodBalance { get; set; }
        public double PeriodBeginningPeriodBalance { get; set; }
        public double PeriodBalanceDifference { get; set; }


        public double PeriodBeginningPeriodExpensesAmount { get; set; }
        public double CurrentPeriodExpensesAmount { get; set; }
        public double PeriodExpensesDifference { get; set; }

        public double PeriodBeginningPeriodIncomesAmount { get; set; }
        public double CurrentPeriodIncomesAmount { get; set; }
        public double PeriodIncomesDifference { get; set; }

        public PeriodOperationsViewModel OperationsModel { get; set; }

        public MoneyOperationModel NewMoneyOperation { get; set; }
        public double NextPeriodBeginningTotalBalance { get; set; }
        public double PeriodEndPeriodExpensesAmount { get; set; }

    }
}