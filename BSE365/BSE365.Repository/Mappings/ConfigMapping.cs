using BSE365.Model.Entities;
using BSE365.Repository.Extensions;
using System.Data.Entity.ModelConfiguration;

namespace BSE365.Model.Mappings
{
    class ConfigMapping: EntityTypeConfiguration<Config>
    {
        public ConfigMapping()
        {            
            Property(u => u.Key).UniqueIndex("Unq_LoginName", 1);
        }
    }
}
