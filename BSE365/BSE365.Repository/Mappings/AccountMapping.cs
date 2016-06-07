using System.Data.Entity.ModelConfiguration;
using BSE365.Model.Entities;

namespace BSE365.Repository.Mappings
{
    public class AccountMapping : EntityTypeConfiguration<Account>
    {
        public AccountMapping()
        {
            HasKey(a => a.UserName);
            HasRequired(a => a.UserInfo)
                .WithMany(u => u.Accounts)
                .HasForeignKey(a => a.UserInfoId)
                .WillCascadeOnDelete(false);
        }
    }
}