using System;
using BSE365.Model.Enum;

namespace BSE365.ViewModels
{
    public class TradeUserInfoVM
    {
        public int Id { get; set; }

        #region user's infomations

        public string DisplayName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string BankNumber { get; set; }

        public string BankName { get; set; }

        public string BankBranch { get; set; }

        public int? AvatarId { get; set; }

        public string AvatarUrl
        {
            get
            {
                var result = ImageViewModel.ImageUrl + (AvatarId ?? 0);
                return result;
            }
        }

        public string UserPrefix { get; set; }

        public DateTime Created { get; set; }

        #endregion

        #region trade account infomations

        public DateTime LastGiveDate { get; set; }

        public int GiveOver { get; set; }

        public int TotalGiveCount { get; set; }

        public int Rating { get; set; }

        public UserState State { get; set; }

        public bool IsAllowAbandonOne { get; set; }

        public string RelatedAccount { get; set; }

        #endregion

        #region treepath and bonus informations

        public string ParentId { get; set; }

        public string TreePath { get; set; }

        public int Level { get; set; }

        public int BonusPoint { get; set; }

        public int TotalBonusPoint { get; set; }

        /// <summary>
        /// temp to count bonus point per day
        /// </summary>
        public DateTime DayBonusTemp { get; set; }

        /// <summary>
        /// temp to count bonus point per day
        /// </summary>
        public int DayBonusPoint { get; set; }

        /// <summary>
        /// last time using bonus point
        /// can't claim 2 times in one day
        /// </summary>
        public DateTime LastClaimBonusDate { get; set; }

        #endregion
    }
}