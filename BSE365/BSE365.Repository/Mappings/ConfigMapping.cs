using System.Data.Entity.ModelConfiguration;
using BSE365.Model.Entities;
using BSE365.Repository.Extensions;

namespace BSE365.Repository.Mappings
{
    internal class ConfigMapping : EntityTypeConfiguration<Config>
    {
        public ConfigMapping()
        {
            Property(u => u.Key).UniqueIndex("Unq_LoginName", 1);
        }
    }
}