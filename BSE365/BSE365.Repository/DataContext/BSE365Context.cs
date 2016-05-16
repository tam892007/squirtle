using BSE365.Model.Entities;
using BSE365.Model.Mappings;
using System.Data.Entity;

namespace BSE365.Repository.DataContext
{
    public class BSE365Context : BSE365.Base.DataContext.DataContext
    {
        public BSE365Context() : base("name=bse365.connection")
        {
            Configuration.LazyLoadingEnabled = true;
        }

        public virtual DbSet<Config> Configs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ConfigMapping());

            base.OnModelCreating(modelBuilder);
        }
    }
}
