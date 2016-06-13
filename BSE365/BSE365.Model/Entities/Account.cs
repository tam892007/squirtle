﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using BSE365.Base.Infrastructures;
using BSE365.Common.Constants;
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

        public virtual UserInfo UserInfo { get; set; }

        public virtual ICollection<MoneyTransaction> Gave { get; set; }
        public virtual ICollection<MoneyTransaction> Received { get; set; }
        public virtual ICollection<WaitingGiver> WaitingGivers { get; set; }
        public virtual ICollection<WaitingReceiver> WaitingReceivers { get; set; }

        [NotMapped]
        public ObjectState ObjectState { get; set; }

        /// <summary>
        ///     Notify account when somethings happend
        /// </summary>
        public void Notify()
        {
        }

        #region state

        public bool IsAllowAbadonTransaction()
        {
            return UserInfo.State == UserState.Default &&
                   UserInfo.IsAllowAbandonOne;
        }

        public bool IsAllowChangeState()
        {
            return UserInfo.State == UserState.Default &&
                   State != AccountState.InGiveTransaction &&
                   State != AccountState.InReceiveTransaction;
        }

        public void ChangeState(AccountState state)
        {
            var now = DateTime.Now.Date;
            if (IsAllowChangeState())
            {
                switch (state)
                {
                    case AccountState.Default:
                        ClearQueued();
                        State = AccountState.Default;
                        var dayFromLastCycle = (now - LastCycleDate).Days;
                        if (dayFromLastCycle < 7)
                        {
                            LastCycleDate = now.AddDays(-8);
                        }
                        ObjectState = ObjectState.Modified;

                        var dayFromLastGive = (now - UserInfo.LastGiveDate).Days;
                        if (dayFromLastGive < 1)
                        {
                            UserInfo.LastGiveDate = now.AddDays(-2);
                            UserInfo.ObjectState = ObjectState.Modified;
                        }
                        break;
                    case AccountState.Gave:
                        ClearQueued();
                        State = AccountState.Gave;
                        ObjectState = ObjectState.Modified;

                        while (UserInfo.GiveOver < 2)
                        {
                            UserInfo.GiveOver += 2;
                            UserInfo.ObjectState = ObjectState.Modified;
                        }
                        break;
                }
            }
        }

        public void ClearQueued()
        {
            foreach (var waitingGiver in WaitingGivers)
            {
                waitingGiver.ObjectState = ObjectState.Deleted;
            }
            foreach (var waitingReceiver in WaitingReceivers)
            {
                waitingReceiver.ObjectState = ObjectState.Deleted;
            }
        }

        #endregion

        #region waiting list

        /// <summary>
        /// require userinfo
        /// require userinfo.accounts
        /// </summary>
        /// <returns></returns>
        public bool IsAllowQueueGive()
        {
            var dayFromLastCycle = (DateTime.Now.Date - LastCycleDate).Days;
            return ((dayFromLastCycle >= 7 && State == AccountState.Default) || State == AccountState.AbadonOne) &&
                   UserInfo.IsAllowQueueGive(this);
        }

        public List<string> NotAllowGiveReason()
        {
            var result = UserInfo.NotAllowGiveReason(this);
            var dayFromLastCycle = (DateTime.Now.Date - LastCycleDate).Days;
            if (dayFromLastCycle < 7)
            {
                result.Add("Last receive date is less than 7 days.");
            }
            if (State != AccountState.Default && State != AccountState.AbadonOne)
            {
                result.Add("Account's State not allowed.");
            }
            return result;
        }

        public bool IsAllowQueueReceive()
        {
            return State == AccountState.Gave && UserInfo.IsAllowQueueReceive();
        }

        public List<string> NotAllowReceiveReason()
        {
            var result = UserInfo.NotAllowReceiveReason();
            if (State != AccountState.Gave)
            {
                result.Add("Account's State not allowed");
            }
            return result;
        }

        /// <summary>
        ///     Queued in WaitingGive
        /// </summary>
        public void QueueGive()
        {
            if (IsAllowQueueGive())
            {
                State = AccountState.WaitGive;
                LastCycleDate = DateTime.Now.Date;
                ObjectState = ObjectState.Modified;

                UserInfo.GiveQueued();

                var giveRequest = new WaitingGiver
                {
                    AccountId = UserName,
                    Priority = Priority,
                    Type = WaitingType.Default,
                    Created = DateTime.Now,
                    ObjectState = ObjectState.Added
                };
                if (State == AccountState.AbadonOne)
                {
                    giveRequest.Type = WaitingType.Abadon;
                    giveRequest.Amount = TransactionConfig.GiveAmountAbadon;
                }
                WaitingGivers.Add(giveRequest);
            }
        }

        /// <summary>
        ///     Queued in WaitingReceive
        /// </summary>
        public void QueueReceive()
        {
            if (IsAllowQueueReceive())
            {
                State = AccountState.WaitReceive;
                ObjectState = ObjectState.Modified;

                UserInfo.ReceiveQueued();

                WaitingReceivers.Add(new WaitingReceiver
                {
                    AccountId = UserName,
                    Priority = Priority,
                    Created = DateTime.Now,
                    ObjectState = ObjectState.Added
                });
            }
        }

        public bool IsAllowClaimBonus()
        {
            return UserInfo.IsAllowClaimBonus();
        }

        public void ClaimBonus()
        {
            if (IsAllowClaimBonus())
            {
                UserInfo.ClaimBonus();

                WaitingReceivers.Add(new WaitingReceiver
                {
                    AccountId = UserName,
                    Priority = Priority,
                    Type = WaitingType.Bonus,
                    Amount = TransactionConfig.ReceiveAmountBonus,
                    Created = DateTime.Now,
                    ObjectState = ObjectState.Added
                });
            }
        }

        #endregion

        #region transaction report methods

        /// <summary>
        ///     A transaction give money successed
        /// </summary>
        public void MoneyGave(MoneyTransaction transaction,
            List<MoneyTransaction> otherGivingTransactionsInCurrentTransaction,
            List<UserInfo> parentInfos)
        {
            if (transaction.Type != TransactionType.Replacement)
            {
                if (otherGivingTransactionsInCurrentTransaction.All(x => x.IsEnd) &&
                    State == AccountState.InGiveTransaction &&
                    WaitingGivers.Count == 0)
                {
                    State = AccountState.Gave;
                    CurrentTransactionGroupId = null;
                    ObjectState = ObjectState.Modified;

                    UserInfo.ResetAbadonStatus();
                }
                UserInfo.MoneyGave(parentInfos);
            }
        }

        /// <summary>
        ///     A transaction receive money successed
        /// </summary>
        public void MoneyReceived(MoneyTransaction transaction,
            List<MoneyTransaction> otherReceivingTransactionsInCurrentTransaction)
        {
            if (transaction.Type != TransactionType.Bonus)
            {
                if (otherReceivingTransactionsInCurrentTransaction.All(x => x.IsEnd) &&
                    State == AccountState.InReceiveTransaction &&
                    WaitingReceivers.Count == 0)
                {
                    State = AccountState.Default;
                    CurrentTransactionGroupId = null;
                    ObjectState = ObjectState.Modified;
                }
            }
        }

        /// <summary>
        /// giver not give in time
        /// </summary>
        /// <param name="transaction"></param>
        public void NotTransfer(MoneyTransaction transaction)
        {
            State = AccountState.NotGive;
            RelatedTransaction = string.Format("{0}{1},", RelatedTransaction, transaction.Id);
            ObjectState = ObjectState.Modified;

            UserInfo.NotTransfer(this);
        }

        /// <summary>
        /// receiver not confirm on time
        /// </summary>
        /// <param name="transaction"></param>
        public void NotConfirm(MoneyTransaction transaction)
        {
            State = AccountState.NotConfirm;
            RelatedTransaction = string.Format("{0}{1},", RelatedTransaction, transaction.Id);
            ObjectState = ObjectState.Modified;

            UserInfo.NotConfirm(this);
        }

        /// <summary>
        /// receiver report giver not give
        /// </summary>
        /// <param name="transaction"></param>
        public void ReportNotTransfer(MoneyTransaction transaction)
        {
            State = AccountState.ReportedNotTransfer;
            ObjectState = ObjectState.Modified;
        }

        /// <summary>
        /// abadon transaction for giver
        /// </summary>
        /// <param name="transaction"></param>
        public void AbadonTransaction(MoneyTransaction transaction)
        {
            if (IsAllowAbadonTransaction())
            {
                State = AccountState.AbadonOne;
                ObjectState = ObjectState.Modified;

                // update user info 
                UserInfo.AbadonTransaction();
            }
        }

        /// <summary>
        /// requeue waiting receiver list with high priority for receiver 
        /// </summary>
        /// <param name="transaction"></param>
        public void ReQueueWaitingListForAbadonTransaction(MoneyTransaction transaction)
        {
            if (WaitingReceivers.Count == 0)
            {
                WaitingReceivers.Add(new WaitingReceiver
                {
                    AccountId = UserName,
                    Priority = PriorityLevel.High,
                    Created = DateTime.Now,
                    Amount = 1,
                    ObjectState = ObjectState.Added
                });
            }
            else
            {
                var waiting = WaitingReceivers.First();
                waiting.Amount++;
                waiting.Priority = PriorityLevel.High;
                waiting.ObjectState = ObjectState.Modified;
                if (waiting.Amount == TransactionConfig.ReceiveAmountDefault)
                {
                    State = AccountState.WaitReceive;
                    ObjectState = ObjectState.Modified;
                }
            }
        }

        #endregion
    }
}