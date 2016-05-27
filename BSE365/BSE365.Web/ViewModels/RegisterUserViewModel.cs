using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSE365.ViewModels
{
    public class RegisterUserViewModel
    {
        public string UserName { get; set; }
        
        public string Password { get; set; }

        public UserInfoViewModel UserInfo { get; set; }
    }
}