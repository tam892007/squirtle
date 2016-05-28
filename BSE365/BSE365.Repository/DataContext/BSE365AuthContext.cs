using BSE365.Model.Entities;
using BSE365.Repository.Mappings;
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
        public virtual DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UserInfoMapping());
            base.OnModelCreating(modelBuilder);
        }
    }
}