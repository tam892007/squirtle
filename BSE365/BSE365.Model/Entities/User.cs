using System;
using System.CodeDom;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace BSE365.Model.Entities
{
    public class User : IdentityUser
    {
        public uint PinBalance { get; set; }

        public virtual UserInfo UserInfo { get; set; }

        public void TransferPin(uint amout)
        {
            this.PinBalance = this.PinBalance - amout;
        }

        public void ReceivePin(uint amout)
        {
            this.PinBalance = this.PinBalance + amout;
        }
    }
}