using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSE365.Model.Enum;

namespace BSE365.Model.Entities
{
    public class WaitingBase : BaseEntity
    {
        public WaitingBase()
        {
            Priority = PriorityLevel.Default;
        }

        public string AccountId { get; set; }
        public PriorityLevel Priority { get; set; }
        public DateTime Created { get; set; }
        public Account Account { get; set; }
    }
}