using BSE365.Common.Constants;
using Hangfire;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSE365.Web
{
    public partial class Startup
    {
        public void ConfigureHangFire(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage(WebConfigKey.ConnectionStringName);
            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}