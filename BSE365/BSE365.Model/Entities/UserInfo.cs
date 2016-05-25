using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE365.Model.Entities
{
    public class UserInfo: BaseEntity
    {
        [Required]   
        public string DisplayName { get; set; }

        [Required]
        public string Email { get; set; }

        public string ParentId { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string BankNumber { get; set; }

        [Required]
        public string BankName { get; set; }

        [Required]
        public string BankBranch { get; set; }

        public int? AvatarId { get; set; }

        public virtual Image Avatar { get; set; }
    }
}
