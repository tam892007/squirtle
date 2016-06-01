using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSE365.Model.Entities;

namespace BSE365.Repository.Mappings
{
    public class MessageMapping : EntityTypeConfiguration<Message>
    {
        public MessageMapping()
        {
            HasRequired(m => m.From)
                .WithMany(a => a.MessageSent)
                .HasForeignKey(m => m.FromId)
                .WillCascadeOnDelete(false);
            HasRequired(m => m.To)
                .WithMany(a => a.MessageReceied)
                .HasForeignKey(m => m.ToId)
                .WillCascadeOnDelete(false);
        }
    }
}