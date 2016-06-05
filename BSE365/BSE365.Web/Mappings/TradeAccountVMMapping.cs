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
                UserName = target.UserName,
                IsAllowGive = target.IsAllowGive(),
                IsAllowReceive = target.IsAllowReceive(),
                State = target.State,
                Priority = target.Priority,
                LastCycleDate = target.LastCycleDate,
                CurrentTransactionGroupId = target.CurrentTransactionGroupId,
                RelatedTransaction = target.RelatedTransaction,
                DisplayName = target.UserInfo.DisplayName,
                UserState = target.UserInfo.State,
                IsAllowAbandonOne = target.UserInfo.IsAllowAbandonOne,
                RelatedAccount = target.UserInfo.RelatedAccount,
                AvatarId = target.UserInfo.AvatarId,
                ParentId = target.UserInfo.ParentId,
                PhoneNumber = target.UserInfo.PhoneNumber,
                Email = target.UserInfo.Email,
                BankNumber = target.UserInfo.BankNumber,
                BankName = target.UserInfo.BankName,
                BankBranch = target.UserInfo.BankBranch,
                Rating = target.UserInfo.Rating,
                Level = target.UserInfo.Level,
                TreePath = target.UserInfo.TreePath,
            };
            return result;
        }

        public static Expression<Func<Account, TradeAccountVM>> GetExpToVM()
        {
            Expression<Func<Account, TradeAccountVM>> result = x => new TradeAccountVM
            {
                //IsAllowGive = x.IsAllowGive(),
                //IsAllowReceive = x.IsAllowReceive(),
                UserName = x.UserName,
                State = x.State,
                Priority = x.Priority,
                LastCycleDate = x.LastCycleDate,
                CurrentTransactionGroupId = x.CurrentTransactionGroupId,
                RelatedTransaction = x.RelatedTransaction,
                DisplayName = x.UserInfo.DisplayName,
                UserState = x.UserInfo.State,
                IsAllowAbandonOne = x.UserInfo.IsAllowAbandonOne,
                RelatedAccount = x.UserInfo.RelatedAccount,
                AvatarId = x.UserInfo.AvatarId,
                ParentId = x.UserInfo.ParentId,
                PhoneNumber = x.UserInfo.PhoneNumber,
                Email = x.UserInfo.Email,
                BankNumber = x.UserInfo.BankNumber,
                BankName = x.UserInfo.BankName,
                BankBranch = x.UserInfo.BankBranch,
                Rating = x.UserInfo.Rating,
                Level = x.UserInfo.Level,
                TreePath = x.UserInfo.TreePath,
            };
            return result;
        }
    }
}