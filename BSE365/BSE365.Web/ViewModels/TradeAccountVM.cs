﻿using System;
using System.Collections.Generic;
using BSE365.Model.Enum;

namespace BSE365.ViewModels
{
    public class TradeAccountVM
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }

        public int PinBalance { get; set; }

        public bool IsAllowGive { get; set; }
        public bool IsAllowReceive { get; set; }
        public bool IsAllowChangeState { get; set; }
        public bool IsAllowAbadonTransaction { get; set; }

        public List<string> NotAllowGiveReason { get; set; }
        public List<string> NotAllowReceiveReason { get; set; }

        public AccountState State { get; set; }
        public PriorityLevel Priority { get; set; }
        public DateTime LastCycleDate { get; set; }
        public int? CurrentTransactionGroupId { get; set; }

        /// <summary>
        /// transactions's id what have some problem
        /// </summary>
        public string RelatedTransaction { get; set; }

        public UserState UserState { get; set; }
        public bool IsAllowAbandonOne { get; set; }
        public string RelatedAccount { get; set; }

        public int? AvatarId { get; set; }

        public string Email { get; set; }
        public string ParentId { get; set; }
        public string PhoneNumber { get; set; }
        public string BankNumber { get; set; }
        public string BankName { get; set; }
        public string BankBranch { get; set; }

        public int Rating { get; set; }
        public int Level { get; set; }
        public string TreePath { get; set; }

        public class SetPriorityVM
        {
            public string UserName { get; set; }

            public PriorityLevel Priority { get; set; }
        }

        public class SetStateVM
        {
            public string UserName { get; set; }
            public AccountState State { get; set; }
        }
    }
}