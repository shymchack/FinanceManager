using FinanceManager.BL.UserInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinanceManager.API.Serialization.Types
{
    public class PeriodSummaryModel
    {
        public PeriodSummaryModel()
        {
            OperationsModel = new PeriodOperationsModel();
            NewMoneyOperation = new MoneyOperationModel();
        }

        public string PeriodTitle { get; set; }

        public double CurrentTotalBalance { get; set; }
        public double PeriodBeginningTotalBalance { get { return CurrentTotalBalance + (double)OperationsModel.PeriodOperations.Sum(om => om.CurrentPeriodPayedAmount); } }
        public double TotalBalanceDifference { get { return CurrentTotalBalance - PeriodBeginningTotalBalance; } }


        public double CurrentPeriodBalance { get { return CurrentPeriodIncomesAmount - CurrentPeriodExpensesAmount; } }
        public double PeriodBeginningPeriodBalance { get { return PeriodBeginningPeriodIncomesAmount - PeriodBeginningPeriodExpensesAmount; } }
        public double PeriodBalanceDifference { get { return CurrentPeriodBalance - PeriodBeginningPeriodBalance; } }


        public double PeriodBeginningPeriodExpensesAmount { get { return (double) OperationsModel.PeriodOperations.Sum(mo => mo.CurrentPeriodBeginningBudgetedAmount); } }
        public double CurrentPeriodExpensesAmount { get { return (double) OperationsModel.PeriodOperations.Sum(mo => mo.CurrentPeriodBudgetedAmount); } } //TODO needed to add money operation modification date
        public double PeriodExpensesDifference { get { return CurrentPeriodExpensesAmount - PeriodBeginningPeriodExpensesAmount; } }

        public double PeriodBeginningPeriodIncomesAmount { get; set; }
        public double CurrentPeriodIncomesAmount { get; set; }
        public double PeriodIncomesDifference { get { return CurrentPeriodIncomesAmount - PeriodBeginningPeriodIncomesAmount; } }

        public PeriodOperationsModel OperationsModel { get; set; }

        public MoneyOperationModel NewMoneyOperation { get; set; }
        public double NextPeriodBeginningTotalBalance { get { return PeriodBeginningTotalBalance + (double) OperationsModel.PeriodOperations.Sum(po => po.CurrentPeriodBeginningBudgetedAmount); } }
        public double PeriodEndPeriodExpensesAmount { get { return (double) OperationsModel.PeriodOperations.Sum(mo => mo.CurrentPeriodEndBudgetedAmount); } }
    }
}