using System.Data.Entity.ModelConfiguration;
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