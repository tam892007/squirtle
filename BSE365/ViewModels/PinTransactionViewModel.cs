﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSE365.ViewModels
{
    public class PinTransactionViewModel
    {
        public string FromId { get; set; }
        public string ToId { get; set; }
        public int Amount { get; set; }
        public string Note { get; set; }
        public string Code { get; set; }
    }
}