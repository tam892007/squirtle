using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using BSE365.Model.Entities;
using BSE365.ViewModels;

namespace BSE365.Mappings
{
    public static class TradeUserInfoVMMapping
    {
        public static TradeUserInfoVM ToVM(this UserInfo target)
        {
            var result = new TradeUserInfoVM
            {
                DisplayName = target.DisplayName,
                Email = target.Email,
                PhoneNumber = target.PhoneNumber,
                BankNumber = target.BankNumber,
                BankName = target.BankName,
                BankBranch = target.BankBranch,
                AvatarId = target.AvatarId,
                UserPrefix = target.UserPrefix,
                Created = target.Created,
                LastGiveDate = target.LastGiveDate,
                GiveOver = target.GiveOver,
                TotalGiveCount = target.TotalGiveCount,
                Rating = target.Rating,
                State = target.State,
                IsAllowAbandonOne = target.IsAllowAbandonOne,
                RelatedAccount = target.RelatedAccount,
                ParentId = target.ParentId,
                TreePath = target.TreePath,
                Level = target.Level,
                BonusPoint = target.BonusPoint,
                TotalBonusPoint = target.TotalBonusPoint,
                DayBonusTemp = target.DayBonusTemp,
                DayBonusPoint = target.DayBonusPoint,
                LastClaimBonusDate = target.LastClaimBonusDate,
            };
            return result;
        }

        public static Expression<Func<UserInfo, TradeUserInfoVM>> GetExpToVM()
        {
            Expression<Func<UserInfo, TradeUserInfoVM>> result =
                x => new TradeUserInfoVM
                {
                    DisplayName = x.DisplayName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    BankNumber = x.BankNumber,
                    BankName = x.BankName,
                    BankBranch = x.BankBranch,
                    AvatarId = x.AvatarId,
                    UserPrefix = x.UserPrefix,
                    Created = x.Created,
                    LastGiveDate = x.LastGiveDate,
                    GiveOver = x.GiveOver,
                    TotalGiveCount = x.TotalGiveCount,
                    Rating = x.Rating,
                    State = x.State,
                    IsAllowAbandonOne = x.IsAllowAbandonOne,
                    RelatedAccount = x.RelatedAccount,
                    ParentId = x.ParentId,
                    TreePath = x.TreePath,
                    Level = x.Level,
                    BonusPoint = x.BonusPoint,
                    TotalBonusPoint = x.TotalBonusPoint,
                    DayBonusTemp = x.DayBonusTemp,
                    DayBonusPoint = x.DayBonusPoint,
                    LastClaimBonusDate = x.LastClaimBonusDate,
                };
            return result;
        }
    }
}