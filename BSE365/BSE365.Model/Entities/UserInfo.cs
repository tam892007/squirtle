using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
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

        [Required]
        [StringLength(30)]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(30)]
        public string Email { get; set; }

        public string ParentId { get; set; }

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

        [DataType(DataType.Date)]
        public DateTime LastGiveDate { get; set; }

        public int GiveOver { get; set; }

        public UserState State { get; set; }

        public bool IsAllowAbandonOne { get; set; }

        public string RelatedAccount { get; set; }

        public int? AvatarId { get; set; }

        public virtual Image Avatar { get; set; }

        public int Rating { get; set; }

        public int Level { get; set; }

        public string TreePath { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }

        public bool IsAllowGive()
        {
            var dayFromLastGive = (DateTime.Now - LastGiveDate).Days;
            return dayFromLastGive >= 1;
        }

        public bool IsAllowReceive()
        {
            return GiveOver >= 2;
        }

        /// <summary>
        /// Queued in WaitingGive
        /// </summary>
        public void GiveQueued()
        {
            LastGiveDate = DateTime.Now.Date;
            ObjectState = ObjectState.Modified;
        }

        /// <summary>
        /// Queued in WaitingReceive
        /// </summary>
        public void ReceiveQueued()
        {
            GiveOver -= 2;
            ObjectState = ObjectState.Modified;
        }

        /// <summary>
        /// A transaction give money successed
        /// </summary>
        public void MoneyGave()
        {
            GiveOver += 1;
            ObjectState = ObjectState.Modified;
        }
    }
}