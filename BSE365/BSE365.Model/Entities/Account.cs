using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSE365.Base.DataContext;
using BSE365.Base.Infrastructures;
using BSE365.Model.Enum;

namespace BSE365.Model.Entities
{
    public class Account : IObjectState
    {
        public Account()
        {
            State = AccountState.Default;
            Priority = PriorityLevel.Default;
            LastCycleDate = DateTime.Now.AddDays(-7).Date;

            Gave = new HashSet<MoneyTransaction>();
            Received = new HashSet<MoneyTransaction>();
            WaitingGivers = new HashSet<WaitingGiver>();
            WaitingReceivers = new HashSet<WaitingReceiver>();
        }

        [Required]
        [Key]
        public string UserName { get; set; }

        public AccountState State { get; set; }

        public PriorityLevel Priority { get; set; }

        [DataType(DataType.Date)]
        public DateTime LastCycleDate { get; set; }

        public string RelatedTransaction { get; set; }

        public int? CurrentTransactionGroupId { get; set; }

        public int UserInfoId { get; set; }

        [NotMapped]
        public ObjectState ObjectState { get; set; }

        public virtual UserInfo UserInfo { get; set; }

        public virtual ICollection<MoneyTransaction> Gave { get; set; }
        public virtual ICollection<MoneyTransaction> Received { get; set; }
        public virtual ICollection<WaitingGiver> WaitingGivers { get; set; }
        public virtual ICollection<WaitingReceiver> WaitingReceivers { get; set; }

        public bool IsAllowGive()
        {
            var dayFromLastCycle = (DateTime.Now - LastCycleDate).Days;
            return dayFromLastCycle >= 7 && State == AccountState.Default && UserInfo.IsAllowGive();
        }

        public bool IsAllowReceive()
        {
            return State == AccountState.Gave && UserInfo.IsAllowReceive();
        }

        /// <summary>
        /// Queued in WaitingGive
        /// </summary>
        public void QueueGive()
        {
            if (IsAllowGive())
            {
                State = AccountState.WaitGive;
                LastCycleDate = DateTime.Now.Date;
                ObjectState = ObjectState.Modified;

                UserInfo.GiveQueued();

                WaitingGivers.Add(new WaitingGiver
                {
                    AccountId = UserName,
                    Priority = Priority,
                    Created = DateTime.Now,
                    ObjectState = ObjectState.Added,
                });
            }
        }

        /// <summary>
        /// Queued in WaitingReceive
        /// </summary>
        public void QueueReceive()
        {
            if (IsAllowReceive())
            {
                State = AccountState.WaitReceive;
                ObjectState = ObjectState.Modified;

                UserInfo.ReceiveQueued();

                WaitingReceivers.Add(new WaitingReceiver
                {
                    AccountId = UserName,
                    Priority = Priority,
                    Created = DateTime.Now,
                    ObjectState = ObjectState.Added,
                });
            }
        }

        /// <summary>
        /// A transaction give money successed
        /// </summary>
        public void MoneyGave(MoneyTransaction transaction)
        {
            UserInfo.MoneyGave();
        }

        /// <summary>
        /// A transaction receive money successed
        /// </summary>
        public void MoneyReceived(MoneyTransaction transaction)
        {
        }

        /// <summary>
        /// Notify account when somethings happend
        /// </summary>
        public void Notify()
        {
        }

        public void NotTransfer(MoneyTransaction transaction)
        {
            State = AccountState.NotGive;
            ObjectState = ObjectState.Modified;
        }

        public void NotConfirm(MoneyTransaction transaction)
        {
            State = AccountState.NotConfirm;
            ObjectState = ObjectState.Modified;
        }

        public void ReportNotTransfer(MoneyTransaction transaction)
        {
            State = AccountState.ReportedNotTransfer;
            ObjectState = ObjectState.Modified;
        }
    }
}