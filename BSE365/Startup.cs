using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

[assembly: OwinStartup(typeof(BSE365.Web.Startup))]
namespace BSE365.Web
{    
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            //Unity
            ConfigureUnity(config);

            ////Web Api
            ConfigureWebApi(config);
            app.UseWebApi(config);
        }
    }
}