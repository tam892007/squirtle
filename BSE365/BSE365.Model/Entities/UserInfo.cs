using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BSE365.Base.Infrastructures;
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
            Accounts = new HashSet<Account>();
        }

        public string ParentId { get; set; }

        public int Level { get; set; }

        public string TreePath { get; set; }

        public virtual Image Avatar { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }

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

        #endregion

        #region trade account infomations

        [DataType(DataType.Date)]
        public DateTime LastGiveDate { get; set; }

        public int GiveOver { get; set; }

        public int Rating { get; set; }

        public UserState State { get; set; }

        public bool IsAllowAbandonOne { get; set; }

        public string RelatedAccount { get; set; }

        #endregion

        #region waiting list

        /// <summary>
        /// require accounts
        /// </summary>
        /// <returns></returns>
        public bool IsAllowQueueGive()
        {
            return DateTime.Now.Date > LastGiveDate &&
                   State == UserState.Default &&
                   Accounts.All(x => x.State != AccountState.WaitGive && x.State != AccountState.InGiveTransaction);
        }

        public List<string> NotAllowGiveReason()
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
            return result;
        }

        public bool IsAllowQueueReceive()
        {
            return GiveOver >= 2 && State == UserState.Default;
        }

        public List<string> NotAllowReceiveReason()
        {
            var result = new List<string>();
            if (GiveOver < 2)
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
            GiveOver -= 2;
            ObjectState = ObjectState.Modified;
        }

        #endregion

        #region transaction report methods

        /// <summary>
        ///     A transaction give money successed
        /// </summary>
        public void MoneyGave()
        {
            GiveOver += 1;
            Rating += 1;
            ObjectState = ObjectState.Modified;
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

        #endregion
        
        public void AbadonTransaction()
        {
            IsAllowAbandonOne = false;
            ObjectState = ObjectState.Modified;
        }
    }
}