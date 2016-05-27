using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using BSE365.Model.Entities;
using BSE365.ViewModels;

namespace BSE365.Mappings
{
    public static class TradeAccountVMMapping
    {
        public static TradeAccountVM ToVM(this Account target)
        {
            var result = new TradeAccountVM
            {
                IsAllowGive = target.IsAllowGive(),
                IsAllowReceive = target.IsAllowReceive(),
                State = target.State,
                Priority = target.Priority,
            };
            return result;
        }
    }
}