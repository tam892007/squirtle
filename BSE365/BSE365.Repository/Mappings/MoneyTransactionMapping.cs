using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSE365.Model.Entities;

namespace BSE365.Repository.Mappings
{
    public class MoneyTransactionMapping : EntityTypeConfiguration<MoneyTransaction>
    {
        public MoneyTransactionMapping()
        {
            HasRequired(m => m.Giver)
                .WithMany(u => u.Gave)
                .HasForeignKey(m => m.GiverId)
                .WillCascadeOnDelete(false);
            HasRequired(m => m.Receiver)
                .WithMany(u => u.Received)
                .HasForeignKey(m => m.ReceiverId)
                .WillCascadeOnDelete(false);


            HasOptional(m => m.RelatedTransaction)
                .WithMany()
                .HasForeignKey(m => m.RelatedTransactionId)
                .WillCascadeOnDelete(false);
        }
    }
}