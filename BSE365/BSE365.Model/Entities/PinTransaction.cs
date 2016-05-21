using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE365.Model.Entities
{
    public class PinTransaction : BaseEntity
    {
        public string FromId { get; set; }

        public string ToId { get; set; }

        public int Amount { get; set; }

        public string Note { get; set; }

        public string Code { get; set; }
    }
}
