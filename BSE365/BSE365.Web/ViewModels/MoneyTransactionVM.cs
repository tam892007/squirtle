using System;
using BSE365.Common.Constants;
using BSE365.Model.Enum;

namespace BSE365.ViewModels
{
    public class MoneyTransactionVM
    {
        public class Giver : Simple
        {
        }

        public class Receiver : Simple
        {
        }

        public class Punishment : Simple
        {
            public int? RelatedTransactionId { get; set; }

            public string ForAccount { get; set; }
            public string ForUser { get; set; }
        }

        public abstract class Simple
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

            public int Amount
            {
                get { return TransactionConfig.MoneyPerTransaction; }
            }

            public string MoneyCurrency
            {
                get { return TransactionConfig.MoneyCurrency; }
            }

            public bool IsEnd { get; set; }


            public string DisplayName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string BankNumber { get; set; }
            public string BankName { get; set; }
            public string BankBranch { get; set; }
            public int Rating { get; set; }

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

        public class Base
        {
            public int Id { get; set; }

            public string GiverId { get; set; }
            public string GiverDisplayName { get; set; }
            public string GiverEmail { get; set; }
            public string GiverPhoneNumber { get; set; }
            public string GiverBankNumber { get; set; }
            public string GiverBankName { get; set; }
            public string GiverBankBranch { get; set; }
            public int GiverRating { get; set; }


            public string ReceiverId { get; set; }
            public string ReceiverDisplayName { get; set; }
            public string ReceiverEmail { get; set; }
            public string ReceiverPhoneNumber { get; set; }
            public string ReceiverBankNumber { get; set; }
            public string ReceiverBankName { get; set; }
            public string ReceiverBankBranch { get; set; }
            public int ReceiverRating { get; set; }


            public DateTime Created { get; set; }
            public DateTime LastModified { get; set; }

            public TransactionType Type { get; set; }
            public TransactionState State { get; set; }
            public string AttachmentUrl { get; set; }

            public DateTime? TransferedDate { get; set; }
            public DateTime? ReceivedDate { get; set; }

            public int? WaitingGiverId { get; set; }

            public int? WaitingReceiverId { get; set; }


            public int Amount
            {
                get { return TransactionConfig.MoneyPerTransaction; }
            }

            public string MoneyCurrency
            {
                get { return TransactionConfig.MoneyCurrency; }
            }

            public bool IsEnd { get; set; }

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

            /// <summary>
            /// who quering this data
            /// </summary>
            public string CurrentAccount { get; set; }
        }

        public class Reported : Base
        {
            public ReportResult Result { get; set; }
        }

        public enum ReportResult
        {
            Default = 0,

            /// <summary>
            /// receiver not confirmed
            /// </summary>
            GiverTrue = 1,

            /// <summary>
            /// giver not transfered
            /// </summary>
            ReceiverTrue = 2,

            BothTrue = 11,
            BothFalse = 12,
        }
    }
}