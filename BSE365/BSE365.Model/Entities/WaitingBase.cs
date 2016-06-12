using System;
using BSE365.Model.Enum;

namespace BSE365.Model.Entities
{
    public class WaitingBase : BaseEntity
    {
        public WaitingBase()
        {
            Priority = PriorityLevel.Default;
            Type = WaitingType.Default;
        }

        public string AccountId { get; set; }
        public PriorityLevel Priority { get; set; }
        public WaitingType Type { get; set; }
        public DateTime Created { get; set; }
        public int Amount { get; set; }

        public virtual Account Account { get; set; }
    }
}