using BSE365.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSE365.ViewModels
{
    public class MessageViewModel
    {
        public string Content { get; set; }
        
        public string FromUser { get; set; }

        public string ToUser { get; set; }

        public MessageState Status { get; set; }

        public DateTime SendTime { get; set; }
    }
}