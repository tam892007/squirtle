using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BSE365.Model.Enum;

namespace BSE365.ViewModels
{
    public class TradeAccountVM
    {
        public bool IsAllowGive { get; set; }
        public bool IsAllowReceive { get; set; }

        public AccountState State { get; set; }
        public PriorityLevel Priority { get; set; }
    }
}