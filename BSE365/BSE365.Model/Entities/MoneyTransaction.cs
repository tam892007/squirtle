using System;
using System.Collections.Generic;
using BSE365.Base.Infrastructures;
using BSE365.Model.Enum;

namespace BSE365.Model.Entities
{
    public class MoneyTransaction : BaseEntity
    {
        public MoneyTransaction()
        {
            Type = TransactionType.Default;
            State = TransactionState.Begin;
        }

        public string GiverId { get; set; }
        public string ReceiverId { get; set; }

        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }

        public TransactionType Type { get; set; }
        public TransactionState State { get; set; }
        public string AttachmentUrl { get; set; }

        public DateTime? TransferedDate { get; set; }
        public DateTime? ReceivedDate { get; set; }

        public int WaitingGiverId { get; set; }

        public int WaitingReceiverId { get; set; }


        public int? RelatedTransactionId { get; set; }

        public bool IsEnd { get; set; }

        public virtual Account Giver { get; set; }
        public virtual Account Receiver { get; set; }
        public virtual MoneyTransaction RelatedTransaction { get; set; }

        public void MoneyTransfered(string attachmentUrl)
        {
            State = TransactionState.Transfered;
            TransferedDate = DateTime.Now;
            LastModified = DateTime.Now;
            AttachmentUrl = attachmentUrl;
            ObjectState = ObjectState.Modified;
        }

        public void MoneyReceived(List<MoneyTransaction> otherGivingTransactionsInCurrentTransaction,
            List<MoneyTransaction> otherReceivingTransactionsInCurrentTransaction,
            List<UserInfo> giverParentInfos)
        {
            State = TransactionState.Confirmed;
            ReceivedDate = DateTime.Now;
            LastModified = DateTime.Now;
            IsEnd = true;
            ObjectState = ObjectState.Modified;

            Giver.MoneyGave(this, otherGivingTransactionsInCurrentTransaction, giverParentInfos);
            Receiver.MoneyReceived(this, otherReceivingTransactionsInCurrentTransaction);
        }

        /// <summary>
        /// lock giver account 
        /// create new transaction from giver's parent to receiver
        /// </summary>
        /// <param name="giverParentAccount">giver parent account</param>
        public void NotTransfer(Account giverParentAccount)
        {
            State = TransactionState.NotTransfer;
            LastModified = DateTime.Now;
            IsEnd = true;
            ObjectState = ObjectState.Modified;

            // update giver
            Giver.NotTransfer(this);

            // create replacement transaction to parent
            if (giverParentAccount != null)
            {
                var replacementTransaction = new MoneyTransaction
                {
                    GiverId = giverParentAccount.UserName,
                    ReceiverId = ReceiverId,
                    Created = DateTime.Now,
                    LastModified = DateTime.Now,
                    Type = TransactionType.Replacement,
                    WaitingGiverId = WaitingGiverId,
                    WaitingReceiverId = WaitingReceiverId,
                    RelatedTransactionId = Id,
                    RelatedTransaction = this,
                    ObjectState = ObjectState.Added,
                };
                giverParentAccount.Gave.Add(replacementTransaction);
            }
        }

        /// <summary>
        /// lock receiver account
        /// giver's transaction mark as gave
        /// </summary>
        /// <param name="otherGivingTransactionsInCurrentTransaction"></param>
        /// <param name="giverParentInfos"></param>
        public void NotConfirm(List<MoneyTransaction> otherGivingTransactionsInCurrentTransaction,
            List<UserInfo> giverParentInfos)
        {
            State = TransactionState.NotConfirm;
            LastModified = DateTime.Now;
            IsEnd = true;
            ObjectState = ObjectState.Modified;

            // update receiver
            Receiver.NotConfirm(this);

            // update giver
            Giver.MoneyGave(this, otherGivingTransactionsInCurrentTransaction, giverParentInfos);
        }

        public void ReportNotTransfer()
        {
            State = TransactionState.ReportedNotTransfer;
            LastModified = DateTime.Now;
            ObjectState = ObjectState.Modified;

            //Receiver.ReportNotTransfer(this);
            //Giver.ReportNotTransfer(this);
        }

        public void Abadon()
        {
            Type = TransactionType.Abadoned;
            State = TransactionState.Abadoned;
            LastModified = DateTime.Now;
            IsEnd = true;
            ObjectState = ObjectState.Modified;

            // update giver 
            Giver.AbadonTransaction(this);

            // update receiver
            Receiver.ReQueueWaitingListForAbadonTransaction(this);
        }

        public void Failed()
        {
            State = TransactionState.Failed;
            LastModified = DateTime.Now;
            IsEnd = true;
            ObjectState = ObjectState.Modified;

            // update giver
            Giver.NotTransfer(this);

            // update receiver
            Receiver.NotConfirm(this);
        }
    }
}