using FinanceManager.API.Services;
using FinanceManager.DAL;
using FinanceManager.DAL.Repositories;
using FinanceManager.DAL.Repositories.Contracts;
using Microsoft.Practices.Unity;
using System.Web.Http;
using Unity.WebApi;
using FinanceManager.BL;
using FinanceManager.DAL.UnitOfWork;
using FinanceManager.Database.Context;

namespace FinanceManager.API
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            container.RegisterType<IAccountsService, AccountsService>();
            container.RegisterType<IAccountsRepository, AccountsRepository>();
            container.RegisterType<IUserAccountsUnitOfWork, UserAccountsUnitOfWork>();
            container.RegisterType<IMoneyOperationLogic, MoneyOperationLogic>();
            container.RegisterType<IMoneyOperationsService, MoneyOperationsService>();
            container.RegisterType<IMoneyOperationsUnitOfWork, MoneyOperationsUnitOfWork>();
            container.RegisterType<IMoneyOperationsRepository, MoneyOperationsRepository>();
            container.RegisterType<IFinanceManagerContext, FinanceManagerContext>();
            container.RegisterType<IUsersRepository, UsersRepository>();
            container.RegisterType<IPeriodSummaryService, PeriodSummaryService>();
            container.RegisterType<IPeriodicityLogic, PeriodicityLogic>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }

        public static void RegisterAccountsServiceTestComponents(IAccountsRepository accrepo, IUsersRepository usrepo, IUserAccountsUnitOfWork uow)
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            container.RegisterType<IAccountsService, AccountsService>();
            container.RegisterType<IAccountsRepository, AccountsRepository>();
            container.RegisterInstance(accrepo);
            container.RegisterType<IUsersRepository, UsersRepository>();
            container.RegisterInstance(usrepo);
            container.RegisterType<IUserAccountsUnitOfWork, UserAccountsUnitOfWork>();
            container.RegisterInstance(uow);

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }

        public static void RegisterAccountsServiceTestComponents()
        {

        }

        public static void RegisterTestAccountsRepository()
        {
        }
    }
}