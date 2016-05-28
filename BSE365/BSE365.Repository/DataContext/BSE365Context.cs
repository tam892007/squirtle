using BSE365.Model.Entities;
using System.Data.Entity;
using BSE365.Repository.Mappings;

namespace BSE365.Repository.DataContext
{
    public class BSE365Context : BSE365.Base.DataContext.DataContext
    {
        public BSE365Context() : base("name=bse365.connection")
        {
            Configuration.LazyLoadingEnabled = true;
        }

        public virtual DbSet<Config> Configs { get; set; }
        public virtual DbSet<PinTransactionHistory> PinTransactionHistories { get; set; }
        public virtual DbSet<Image> Images { get; set; }


        public virtual DbSet<UserInfo> UserInfos { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<MoneyTransferGroup> MoneyTransferGroups { get; set; }
        public virtual DbSet<MoneyTransaction> MoneyTransactions { get; set; }
        public virtual DbSet<WaitingGiver> WaitingGivers { get; set; }
        public virtual DbSet<WaitingReceiver> WaitingReceivers { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ConfigMapping());

            modelBuilder.Configurations.Add(new AccountMapping());
            modelBuilder.Configurations.Add(new MoneyTransactionMapping());
            modelBuilder.Configurations.Add(new WaitingGiverMapping());
            modelBuilder.Configurations.Add(new WaitingReceiverMapping());

            base.OnModelCreating(modelBuilder);
        }
    }
}