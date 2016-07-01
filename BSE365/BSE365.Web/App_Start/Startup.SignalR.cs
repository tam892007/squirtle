using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using BSE365.Timers;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Owin;

namespace BSE365.Web
{
    public partial class Startup
    {
        public static void ConfigureSignalR(IAppBuilder app)
        {
            var unityHubActivator = new MvcHubActivator();

            GlobalHost.DependencyResolver.Register(
                typeof(IHubActivator), () => unityHubActivator);

            app.MapSignalR();
        }

        public class MvcHubActivator : IHubActivator
        {
            public IHub Create(HubDescriptor descriptor)
            {
                try
                {
                    return (IHub) DependencyResolver.Current.GetService(descriptor.HubType);
                }
                catch (Exception exception)
                {
                    return null;
                }
            }
        }
    }
}