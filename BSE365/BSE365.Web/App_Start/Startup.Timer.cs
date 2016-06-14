using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using BSE365.Timers;

namespace BSE365.Web
{
    public partial class Startup
    {
        public static void ConfigureTimer(HttpConfiguration config)
        {
            /*var mappingTimer = new MappingTimer();
            mappingTimer.Start();*/

            var transactionTimer = new TransactionTimer();
            transactionTimer.Start();

            var waitingTimer = new WaitingTimer();
            waitingTimer.Start();
        }
    }
}