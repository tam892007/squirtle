using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BSE365.Model.Entities
{
    public class User : IdentityUser
    {
        [Range(0, int.MaxValue)]
        public int PinBalance { get; set; }

        public virtual UserInfo UserInfo { get; set; }

        public void TransferPin(int amout)
        {
            PinBalance = PinBalance - amout;
        }

        public void ReceivePin(int amout)
        {
            PinBalance = PinBalance + amout;
        }

        public void GiveQueued()
        {
            PinBalance--;
        }
    }
}