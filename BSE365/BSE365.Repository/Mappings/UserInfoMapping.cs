using BSE365.Model.Entities;
using BSE365.Repository.Extensions;
using System.Data.Entity.ModelConfiguration;

namespace BSE365.Repository.Mappings
{
    public class UserInfoMapping : EntityTypeConfiguration<UserInfo>
    {
        public UserInfoMapping()
        {
            Property(u => u.BankNumber).UniqueIndex("Unq_BankNumber", 1);
        }
    }
}