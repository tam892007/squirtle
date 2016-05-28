using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSE365.Model.Entities;

namespace BSE365.Repository.Mappings
{
    public class WaitingGiverMapping : EntityTypeConfiguration<WaitingGiver>
    {
        public WaitingGiverMapping()
        {
            HasRequired(r => r.Account)
                .WithMany(a => a.WaitingGivers)
                .HasForeignKey(r => r.AccountId)
                .WillCascadeOnDelete(false);
        }
    }
}