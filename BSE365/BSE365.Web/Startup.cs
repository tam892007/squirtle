using Microsoft.Owin;
using Owin;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

[assembly: OwinStartup(typeof(BSE365.Web.Startup))]
namespace BSE365.Web
{    
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            ////Mvc
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            ////Unity
            ConfigureUnity(config);

            ////OAuth
            ConfigureOAuth(app);

            ////Web Api
            ConfigureWebApi(config);
            app.UseWebApi(config);

            //Database
            DatabaseInitializer();
        }
    }
}