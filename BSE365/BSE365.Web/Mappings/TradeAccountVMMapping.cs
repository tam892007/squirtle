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
                IsAllowGive = target.IsAllowQueueGive(),
                IsAllowReceive = target.IsAllowQueueReceive(),
                NotAllowGiveReason = target.NotAllowGiveReason(),
                NotAllowReceiveReason = target.NotAllowReceiveReason(),
                IsAllowChangeState = target.IsAllowChangeState(),
                IsAllowAbandonTransaction = target.IsAllowAbandonTransaction(),
                IsAllowClaimBonus = target.IsAllowClaimBonus(),
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
                BonusPoint = target.UserInfo.BonusPoint,
                TotalBonusPoint = target.UserInfo.TotalBonusPoint,
                TotalGiveCount = target.UserInfo.TotalGiveCount,
                DayBonusTemp = target.UserInfo.DayBonusTemp,
                DayBonusPoint = target.UserInfo.DayBonusPoint,
                LastClaimBonusDate = target.UserInfo.LastClaimBonusDate,
            };
            return result;
        }

        public static TradeAccountInfoVM ToVM(this User target)
        {
            var result = new TradeAccountInfoVM
            {
                UserName = target.UserName,
                DisplayName = target.UserInfo.DisplayName,
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

        public static Expression<Func<User, TradeAccountInfoVM>> GetExpToVM()
        {
            Expression<Func<User, TradeAccountInfoVM>> result =
                x => new TradeAccountInfoVM
                {
                    UserName = x.UserName,
                    DisplayName = x.UserInfo.DisplayName,
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