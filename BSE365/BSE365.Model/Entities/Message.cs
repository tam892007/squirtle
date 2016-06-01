using BSE365.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE365.Model.Entities
{
    public class Message:BaseEntity
    {
        public string Content { get; set; }

        public string FromId { get; set; }

        public virtual Account From { get; set; }

        public string ToId { get; set; }

        public virtual Account To { get; set; }

        public MessageState Status { get; set; }

        public DateTime SendTime { get; set; }
    }
}
