using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace BSE365.Model.Entities
{
    public class User : IdentityUser
    {
        public virtual UserInfo UserInfo { get; set; }
    }
}
