using System;
using BSE365.Model.Enum;

namespace BSE365.ViewModels
{
    public class WaitingAccountVM
    {
        public int Id { get; set; }

        public string AccountId { get; set; }
        public PriorityLevel Priority { get; set; }
        public DateTime Created { get; set; }
        public int Amount { get; set; }


        public AccountState State { get; set; }
        public DateTime LastCycleDate { get; set; }


        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }


        public int? AvatarId { get; set; }
    }
}