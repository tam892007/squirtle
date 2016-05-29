﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            public string GiverId { get; set; }
            public string ReceiverId { get; set; }

            public DateTime Created { get; set; }
            public DateTime LastModified { get; set; }

            public MoneyTransferState State { get; set; }
            public string AttachmentUrl { get; set; }

            public DateTime? TransferedDate { get; set; }
            public DateTime? ReceivedDate { get; set; }

            public bool IsEnd { get; set; }


            public string DisplayName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string BankNumber { get; set; }
            public string BankName { get; set; }
            public string BankBranch { get; set; }
        }
    }
}