﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSE365.ViewModels
{
    public class UserInfoViewModel
    {
        public string Id { get; set; }

        public string DisplayName { get; set; }
  
        public string Email { get; set; }

        public string ParentId { get; set; }

        public string PhoneNumber { get; set; }

        public string BankNumber { get; set; }

        public string BankName { get; set; }

        public string BankBranch { get; set; }
    }
}