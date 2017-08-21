using FinanceManager.API.Services;
using FinanceManager.DAL;
using FinanceManager.DAL.Repositories;
using FinanceManager.DAL.Repositories.Contracts;
using Microsoft.Practices.Unity;
using System.Web.Http;
using Unity.WebApi;
using System;
using FinanceManager.BL.UserInput;

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