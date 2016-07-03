using Newtonsoft.Json.Serialization;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Mvc;
using BSE365.Handlers;
using Elmah.Contrib.WebApi;

namespace BSE365.Web
{
    public partial class Startup
    {
        public static void ConfigureWebApi(HttpConfiguration config)
        {
            // Web API configuration and services
            config.MapHttpAttributeRoutes();

            var formatter = config.Formatters.JsonFormatter;
            formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Web API routes
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new {id = RouteParameter.Optional}
                );

            // enable elmah
            config.Services.Add(typeof(IExceptionLogger), new ElmahExceptionLogger());
            config.Filters.Add(new ElmahHandleErrorApiAttribute());

            //log4net
            log4net.Config.XmlConfigurator.Configure();
            config.MessageHandlers.Add(new LogRequestAndResponseHandler());
        }
    }
}