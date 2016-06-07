using System.Data.Entity.ModelConfiguration;
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