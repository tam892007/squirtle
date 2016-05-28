using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSE365.Base.Infrastructures;
using BSE365.Model.Enum;

namespace BSE365.Model.Entities
{
    public class MoneyTransaction : BaseEntity
    {
        public string GiverId { get; set; }
        public string ReceiverId { get; set; }

        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }

        public MoneyTransferState State { get; set; }
        public string AttachmentUrl { get; set; }

        public DateTime? TransferedDate { get; set; }
        public DateTime? ReceivedDate { get; set; }

        public int MoneyTransferGroupId { get; set; }

        public bool IsEnd { get; set; }

        public virtual Account Giver { get; set; }
        public virtual Account Receiver { get; set; }

        public void MoneyTranfered(string attachmentUrl)
        {
            State = MoneyTransferState.Transfered;
            TransferedDate = DateTime.Now;
            LastModified = DateTime.Now;
            AttachmentUrl = attachmentUrl;
            ObjectState = ObjectState.Modified;

            Receiver.Notify();
        }

        public void MoneyReceived()
        {
            State = MoneyTransferState.Confirmed;
            ReceivedDate = DateTime.Now;
            LastModified = DateTime.Now;
            IsEnd = true;
            ObjectState = ObjectState.Modified;

            Giver.MoneyGave(this);
            Receiver.MoneyReceived(this);
        }

        public void NotTransfer()
        {
            State = MoneyTransferState.NotTransfer;
            LastModified = DateTime.Now;
            ObjectState = ObjectState.Modified;

            Giver.NotTransfer(this);
        }

        public void NotConfirm()
        {
            State = MoneyTransferState.NotConfirm;
            LastModified = DateTime.Now;
            ObjectState = ObjectState.Modified;

            Receiver.NotConfirm(this);
        }

        public void ReportNotTransfer()
        {
            State = MoneyTransferState.ReportedNotTransfer;
            LastModified = DateTime.Now;
            ObjectState = ObjectState.Modified;

            Receiver.ReportNotTransfer(this);
        }
    }
}