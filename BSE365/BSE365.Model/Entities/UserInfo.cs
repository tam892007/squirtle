﻿using System;
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
    }
}
