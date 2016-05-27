using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSE365.Model.Entities;

namespace BSE365.Repository.Mappings
{
    public class WaitingReceiverMapping : EntityTypeConfiguration<WaitingReceiver>
    {
        public WaitingReceiverMapping()
        {
            HasRequired(g => g.Account)
                .WithMany(a => a.WaitingReceivers)
                .HasForeignKey(g => g.AccountId)
                .WillCascadeOnDelete(false);
        }
    }
}