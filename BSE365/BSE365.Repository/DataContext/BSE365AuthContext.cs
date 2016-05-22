using BSE365.Model.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace BSE365.Repository.DataContext
{
    public class BSE365AuthContext : IdentityDbContext<User>
    {
        public BSE365AuthContext() : base("name=bse365.connection")
        {

        }

        public virtual DbSet<UserInfo> UserInfos { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<PinTransactionHistory> PinTransactionHistories { get; set; }
    }
}
