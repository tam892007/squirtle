using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using BSE365.Model.Entities;
using BSE365.ViewModels;

namespace BSE365.Mappings
{
    public static class WaitingAccountVMMapping
    {
        public static Expression<Func<WaitingGiver, WaitingAccountVM>> GetExpToGiverVM()
        {
            Expression<Func<WaitingGiver, WaitingAccountVM>> result =
                x => new WaitingAccountVM
                {
                    Id = x.Id,
                    AccountId = x.AccountId,
                    Priority = x.Priority,
                    Created = x.Created,
                    //Amount = x.Amount,
                    State = x.Account.State,
                    LastCycleDate = x.Account.LastCycleDate,
                    DisplayName = x.Account.UserInfo.DisplayName,
                    Email = x.Account.UserInfo.Email,
                    PhoneNumber = x.Account.UserInfo.PhoneNumber,
                    //AvatarUrl = x.Account.UserInfo.AvatarId,
                };
            return result;
        }

        public static Expression<Func<WaitingReceiver, WaitingAccountVM>> GetExpToReceiverVM()
        {
            Expression<Func<WaitingReceiver, WaitingAccountVM>> result =
                x => new WaitingAccountVM
                {
                    Id = x.Id,
                    AccountId = x.AccountId,
                    Priority = x.Priority,
                    Created = x.Created,
                    Amount = x.Amount,
                    State = x.Account.State,
                    LastCycleDate = x.Account.LastCycleDate,
                    DisplayName = x.Account.UserInfo.DisplayName,
                    Email = x.Account.UserInfo.Email,
                    PhoneNumber = x.Account.UserInfo.PhoneNumber,
                    //AvatarUrl = x.Account.UserInfo.AvatarId,
                };
            return result;
        }
    }
}