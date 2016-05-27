using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSE365.Model.Entities;

namespace BSE365.Repository.Mappings
{
    public class UserInfoMapping : EntityTypeConfiguration<UserInfo>
    {
        public UserInfoMapping()
        {
        }
    }
}