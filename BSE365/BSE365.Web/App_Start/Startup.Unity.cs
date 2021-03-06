﻿using BSE365.Base.DataContext.Contracts;
using BSE365.Base.Repositories;
using BSE365.Base.Repositories.Contracts;
using BSE365.Base.UnitOfWork;
using BSE365.Base.UnitOfWork.Contracts;
using BSE365.Model.Entities;
using BSE365.Repository.DataContext;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.WebApi;
using System;
using System.Web.Http;
using BSE365.Repository.Repositories;
using System.Web.Mvc;

namespace BSE365.Web
{
    public partial class Startup
    {
        #region Unity Container

        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }

        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            container
                //.RegisterType<IDataContextAsync, BSE365Context>(new HierarchicalLifetimeManager())
                //.RegisterType<IUnitOfWorkAsync, UnitOfWork>(new HierarchicalLifetimeManager())
                .RegisterType<IDataContextAsync, BSE365Context>(new PerRequestLifetimeManager())
                .RegisterType<IUnitOfWorkAsync, UnitOfWork>(new PerRequestLifetimeManager())
                .RegisterType<IRepositoryAsync<Image>, Repository<Image>>()
                .RegisterType<IRepositoryAsync<PinTransactionHistory>, Repository<PinTransactionHistory>>()
                .RegisterType<IRepositoryAsync<Config>, Repository<Config>>()
                .RegisterType<IRepositoryAsync<Account>, Repository<Account>>()
                .RegisterType<IRepositoryAsync<UserInfo>, Repository<UserInfo>>()
                .RegisterType<IRepositoryAsync<Message>, Repository<Message>>()
                .RegisterType<IRepositoryAsync<WaitingGiver>, Repository<WaitingGiver>>()
                .RegisterType<IRepositoryAsync<WaitingReceiver>, Repository<WaitingReceiver>>()
                .RegisterType<IRepositoryAsync<MoneyTransaction>, Repository<MoneyTransaction>>()
                .RegisterType<IRepositoryAsync<MoneyTransferGroup>, Repository<MoneyTransferGroup>>()
                .RegisterType<IMoneyRepository, MoneyRepository>();
        }

        public static void ConfigureUnity(HttpConfiguration config)
        {
            DependencyResolver.SetResolver(new Unity.Mvc5.UnityDependencyResolver(GetConfiguredContainer()));

            var resolver = new UnityDependencyResolver(GetConfiguredContainer());
            config.DependencyResolver = resolver;
        }
    }
}