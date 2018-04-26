using FinanceManager.BL.UserInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinanceManager.API.Serialization.Types
{
    public class PeriodSummaryModel
    {
        public string PeriodTitle { get; set; }

        public double CurrentTotalBalance { get; set; }
        public double PeriodBeginningTotalBalance { get; set; }
        public double TotalBalanceDifference { get { return CurrentTotalBalance - PeriodBeginningTotalBalance; } }


        public double CurrentPeriodBalance { get { return CurrentPeriodIncomesAmount - CurrentPeriodExpensesAmount; } }
        public double PeriodBeginningPeriodBalance { get { return PeriodBeginningPeriodIncomesAmount - PeriodBeginningPeriodExpensesAmount; } }
        public double PeriodBalanceDifference { get { return CurrentPeriodBalance - PeriodBeginningPeriodBalance; } }


        public double PeriodBeginningPeriodExpensesAmount { get; set; }
        public double CurrentPeriodExpensesAmount { get; set; }
        public double PeriodExpensesDifference { get { return CurrentPeriodExpensesAmount - PeriodBeginningPeriodExpensesAmount; } }

        public double PeriodBeginningPeriodIncomesAmount { get; set; }
        public double CurrentPeriodIncomesAmount { get; set; }
        public double PeriodIncomesDifference { get { return CurrentPeriodIncomesAmount - PeriodBeginningPeriodIncomesAmount; } }

        public PeriodOperationsModel OperationsModel { get; set; }

        public MoneyOperationModel NewMoneyOperation { get; set; }
        public int NextPeriodBeginningTotalBalance { get; set; }
    }
}