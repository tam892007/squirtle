using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using BSE365.Base.Infrastructures;
using BSE365.Common.Constants;
using BSE365.Model.Enum;

namespace BSE365.Model.Entities
{
    public class UserInfo : BaseEntity
    {
        public UserInfo()
        {
            State = UserState.Default;
            GiveOver = -1;
            LastGiveDate = DateTime.Now.AddDays(-1).Date;
            DayBonusTemp = DateTime.Now.AddDays(-1).Date;
            LastClaimBonusDate = DateTime.Now.AddDays(-1).Date;
            Created = DateTime.Now;
            Accounts = new HashSet<Account>();
        }

        #region user's infomations

        [Required]
        [StringLength(30)]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(30)]
        public string Email { get; set; }

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(30)]
        public string BankNumber { get; set; }

        [Required]
        [StringLength(30)]
        public string BankName { get; set; }

        [Required]
        [StringLength(30)]
        public string BankBranch { get; set; }

        public int? AvatarId { get; set; }

        public string UserPrefix { get; set; }

        public DateTime Created { get; set; }

        #endregion

        #region trade account infomations

        [DataType(DataType.Date)]
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
        [DataType(DataType.Date)]
        public DateTime DayBonusTemp { get; set; }

        /// <summary>
        /// temp to count bonus point per day
        /// </summary>
        public int DayBonusPoint { get; set; }

        /// <summary>
        /// last time using bonus point
        /// can't claim 2 times in one day
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime LastClaimBonusDate { get; set; }

        #endregion

        public virtual Image Avatar { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }

        #region waiting list

        /// <summary>
        /// require accounts
        /// </summary>
        /// <returns></returns>
        public bool IsAllowQueueGive(Account account)
        {
            return DateTime.Now.Date > LastGiveDate &&
                   State == UserState.Default &&
                   Accounts.All(x => x.State != AccountState.WaitGive && x.State != AccountState.InGiveTransaction) &&
                   Accounts.Where(x => x.UserName != account.UserName).All(x => x.State != AccountState.AbandonOne);
        }

        public List<string> NotAllowGiveReason(Account account)
        {
            var result = new List<string>();
            if (DateTime.Now.Date <= LastGiveDate)
            {
                result.Add("Last give is today.");
            }
            if (State != UserState.Default)
            {
                result.Add("User's State not allowed.");
            }
            if (!Accounts.All(x => x.State != AccountState.WaitGive && x.State != AccountState.InGiveTransaction))
            {
                result.Add("Have another Account in Queue or Transaction.");
            }
            if (Accounts.Where(x => x.UserName != account.UserName).Any(x => x.State == AccountState.AbandonOne))
            {
                result.Add("Have another Account abandon one Transaction.");
            }
            return result;
        }

        public bool IsAllowQueueReceive()
        {
            return GiveOver >= TransactionConfig.GiveOverToQueueReceive && State == UserState.Default;
        }

        public List<string> NotAllowReceiveReason()
        {
            var result = new List<string>();
            if (GiveOver < TransactionConfig.GiveOverToQueueReceive)
            {
                result.Add("You must use your another secondary account to complete one more give!");
            }
            if (State != UserState.Default)
            {
                result.Add("User's State not allowed.");
            }
            return result;
        }

        /// <summary>
        ///     Queued in WaitingGive
        /// </summary>
        public void GiveQueued()
        {
            LastGiveDate = DateTime.Now.Date;
            ObjectState = ObjectState.Modified;
        }

        /// <summary>
        ///     Queued in WaitingReceive
        /// </summary>
        public void ReceiveQueued()
        {
            GiveOver -= TransactionConfig.GiveOverToQueueReceive;
            ObjectState = ObjectState.Modified;
        }

        #endregion

        #region transaction report methods

        /// <summary>
        ///     A transaction give money successed
        /// </summary>
        public void MoneyGave(List<UserInfo> parentInfos)
        {
            Rating += 1;

            GiveOver += 1;
            TotalGiveCount += 1;

            ObjectState = ObjectState.Modified;

            AddBonusPointToParents(parentInfos);
        }

        /// <summary>
        /// giver account not give
        /// </summary>
        /// <param name="account"></param>
        public void NotTransfer(Account account)
        {
            State = UserState.NotGive;
            RelatedAccount = string.Format("{0}{1},", RelatedAccount, account.UserName);
            ObjectState = ObjectState.Modified;
        }

        /// <summary>
        /// receiver account not confirm 
        /// </summary>
        /// <param name="account"></param>
        public void NotConfirm(Account account)
        {
            State = UserState.NotConfirm;
            RelatedAccount = string.Format("{0}{1},", RelatedAccount, account.UserName);
            ObjectState = ObjectState.Modified;
        }

        public void AbandonTransaction()
        {
            IsAllowAbandonOne = false;
            ObjectState = ObjectState.Modified;
        }

        public void ResetAbandonStatus()
        {
            IsAllowAbandonOne = true;
            ObjectState = ObjectState.Modified;
        }

        #endregion

        #region bonus

        public bool IsAllowClaimBonus()
        {
            return State == UserState.Default &&
                   LastClaimBonusDate < DateTime.Now.Date &&
                   BonusPoint >= TransactionConfig.BonusPointToExchange;
        }

        public void ClaimBonus()
        {
            BonusPoint -= TransactionConfig.BonusPointToExchange;
            LastClaimBonusDate = DateTime.Now.Date;
            ObjectState = ObjectState.Modified;
        }

        private void AddBonusPointToParents(List<UserInfo> parentInfos)
        {
            parentInfos.Reverse();
            for (int i = 0; i < parentInfos.Count; i++)
            {
                if (i >= TransactionConfig.BonusMaxLevel)
                {
                    break;
                }
                var bonusPoint = i == 0
                    ? TransactionConfig.BonusPointToParent
                    : TransactionConfig.BonusPointToTreePath;
                var userInfo = parentInfos[i];
                userInfo.AddBonusPoint(bonusPoint);
            }
        }

        private void AddBonusPoint(int point)
        {
            if (DayBonusTemp < DateTime.Now.Date)
            {
                DayBonusTemp = DateTime.Now.Date;
                DayBonusPoint = 0;
            }
            DayBonusPoint += point;
            BonusPoint += point;
            TotalBonusPoint += point;
            ObjectState = ObjectState.Modified;
        }

        #endregion
    }
}