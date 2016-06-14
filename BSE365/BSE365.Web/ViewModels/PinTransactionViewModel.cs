using System;

namespace BSE365.ViewModels
{
    public class PinTransactionViewModel
    {
        public string FromId { get; set; }
        public string FromName { get; set; }
        public string ToId { get; set; }
        public int Amount { get; set; }
        public string Note { get; set; }
        public CaptchaViewModel Code { get; set; }

    }

    public class PinTransactionHistoryViewModel : PinTransactionViewModel
    {
        public PinTransactionType Type { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public enum PinTransactionType
    {
        Give = 1,
        Receive = 2
    }
}