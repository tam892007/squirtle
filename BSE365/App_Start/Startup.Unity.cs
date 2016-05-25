using BSE365.Base.DataContext.Contracts;
using BSE365.Base.Repositories;
using BSE365.Base.Repositories.Contracts;
using BSE365.Base.UnitOfWork;
using BSE365.Base.UnitOfWork.Contracts;
using BSE365.Model.Entities;
using BSE365.Repository.DataContext;
using Microsoft.Practices.Unity;
using System;
using System.Web.Http;

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
                .RegisterType<IDataContextAsync, BSE365Context>(new HierarchicalLifetimeManager())
                .RegisterType<IUnitOfWorkAsync, UnitOfWork>(new HierarchicalLifetimeManager())
                .RegisterType<IRepositoryAsync<PinTransactionHistory>, Repository<PinTransactionHistory>>()
                .RegisterType<IRepositoryAsync<Config>, Repository<Config>>()
                .RegisterType<IRepositoryAsync<Image>, Repository<Image>>();
        }

        public static void ConfigureUnity(HttpConfiguration config)
        {
            ////MVC goes first
            System.Web.Mvc.DependencyResolver.SetResolver(new Unity.Mvc5.UnityDependencyResolver(GetConfiguredContainer()));

            var resolver = new Microsoft.Practices.Unity.WebApi.UnityDependencyResolver(GetConfiguredContainer());
            config.DependencyResolver = resolver;

           
        }
    }
}