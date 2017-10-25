﻿using FinanceManager.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinanceManager.Web.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController()
        {

        }

        public ActionResult Index()
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

            MonthOperationViewModel op = new MonthOperationViewModel();
            op.TotalAmount = 1600;
            op.AlreadyPayedAmount = 0;
            op.FinishDate = new DateTime(2018, 03, 31);
            op.Name = "OC Volvo";
            MonthOperationViewModel op2 = new MonthOperationViewModel();
            op2.TotalAmount = 300;
            op2.AlreadyPayedAmount = 0;
            op2.FinishDate = new DateTime(2018, 09, 30);
            op2.Name = "Rocznica";

            model.OperationsModel = new MonthOperationsTableViewModel();
            model.OperationsModel.MonthOperations = new List<MonthOperationViewModel>(new [] { op, op2 });

            model.OperationsModel.AlreadyPayedLabel = "Already payed";
            model.OperationsModel.FinishDateLabel = "Finish date";
            model.OperationsModel.NameLabel = "Name";
            model.OperationsModel.PaymentLeftLabel = "Payment left";
            model.OperationsModel.TotalAmonutLabel = "Total Amount";

            model.NewMoneyOperation.AccountID = 3;

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}