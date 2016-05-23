﻿using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace BSE365.Model.Entities
{
    public class User : IdentityUser
    {
        public virtual UserInfo UserInfo { get; set; }

        public int PinBalance { get; set; }

        public void TransferPin(int amout)
        {
            this.PinBalance = this.PinBalance - amout;
        }

        public void ReceivePin(int amout)
        {
            this.PinBalance = this.PinBalance + amout;
        }
    }
}
