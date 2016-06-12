using System;
using System.Collections.Generic;
using BSE365.Common.Constants;
using BSE365.Model.Enum;

namespace BSE365.ViewModels
{
    public class TradeAccountVM
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }

        public int PinBalance { get; set; }

        public bool IsAllowGive { get; set; }
        public bool IsAllowReceive { get; set; }
        public bool IsAllowChangeState { get; set; }
        public bool IsAllowAbadonTransaction { get; set; }
        public bool IsAllowClaimBonus { get; set; }

        public List<string> NotAllowGiveReason { get; set; }
        public List<string> NotAllowReceiveReason { get; set; }

        public AccountState State { get; set; }
        public PriorityLevel Priority { get; set; }
        public DateTime LastCycleDate { get; set; }
        public int? CurrentTransactionGroupId { get; set; }

        /// <summary>
        /// transactions's id what have some problem
        /// </summary>
        public string RelatedTransaction { get; set; }

        public UserState UserState { get; set; }
        public bool IsAllowAbandonOne { get; set; }
        public string RelatedAccount { get; set; }

        public int? AvatarId { get; set; }

        public string AvatarUrl
        {
            get
            {
                var result = ImageViewModel.ImageUrl + (AvatarId ?? 0);
                return result;
            }
        }

        public string Email { get; set; }
        public string ParentId { get; set; }
        public string PhoneNumber { get; set; }
        public string BankNumber { get; set; }
        public string BankName { get; set; }
        public string BankBranch { get; set; }

        public int Rating { get; set; }
        public int Level { get; set; }
        public string TreePath { get; set; }

        public int BonusPoint { get; set; }
        public int TotalBonusPoint { get; set; }
        public int TotalGiveCount { get; set; }

        public int BonusMoney
        {
            get { return BonusPoint*TransactionConfig.MoneyPerTransaction/100; }
        }

        public DateTime DayBonusTemp { get; set; }

        public int DayBonusPoint { get; set; }

        public DateTime LastClaimBonusDate { get; set; }

        public int MoneyPerTransaction
        {
            get { return TransactionConfig.MoneyPerTransaction; }
        }

        public string MoneyCurrency
        {
            get { return TransactionConfig.MoneyCurrency; }
        }


        public class SetPriorityVM
        {
            public string UserName { get; set; }

            public PriorityLevel Priority { get; set; }
        }

        public class SetStateVM
        {
            public string UserName { get; set; }
            public AccountState State { get; set; }
        }
    }
}