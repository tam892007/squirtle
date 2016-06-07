using System;
using BSE365.Common.Constants;
using BSE365.Model.Enum;

namespace BSE365.ViewModels
{
    public class MoneyTransactionVM
    {
        public class Giver : Base
        {
        }

        public class Receiver : Base
        {
        }

        public abstract class Base
        {
            public int Id { get; set; }

            public string GiverId { get; set; }
            public string ReceiverId { get; set; }

            public DateTime Created { get; set; }
            public DateTime LastModified { get; set; }

            public TransactionType Type { get; set; }
            public TransactionState State { get; set; }
            public string AttachmentUrl { get; set; }

            public DateTime? TransferedDate { get; set; }
            public DateTime? ReceivedDate { get; set; }

            public int? WaitingGiverId { get; set; }

            public int? WaitingReceiverId { get; set; }


            public int? RelatedTransactionId { get; set; }


            public bool IsEnd { get; set; }


            public string DisplayName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string BankNumber { get; set; }
            public string BankName { get; set; }
            public string BankBranch { get; set; }

            public int Countdown
            {
                get
                {
                    var now = DateTime.Now;
                    var result = 0;
                    if (State == TransactionState.Begin)
                    {
                        var endTime = Created.AddHours(TransactionConfig.TimeForEachStepInHours);
                        if (endTime > now)
                        {
                            var timeLeft = endTime - now;
                            result = (int) timeLeft.TotalSeconds;
                        }
                    }
                    else if (State == TransactionState.Transfered)
                    {
                        var endTime = TransferedDate.Value.AddHours(TransactionConfig.TimeForEachStepInHours);
                        if (endTime > now)
                        {
                            var timeLeft = endTime - now;
                            result = (int) timeLeft.TotalSeconds;
                        }
                    }
                    return result;
                }
            }
        }
    }
}