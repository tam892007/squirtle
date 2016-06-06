namespace BSE365.Repository.BSE365ContextMigration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTransactionFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MoneyTransactions", "WaitingGiverId", c => c.Int(nullable: false));
            AddColumn("dbo.MoneyTransactions", "WaitingReceiverId", c => c.Int(nullable: false));
            DropColumn("dbo.MoneyTransactions", "MoneyTransferGroupId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MoneyTransactions", "MoneyTransferGroupId", c => c.Int(nullable: false));
            DropColumn("dbo.MoneyTransactions", "WaitingReceiverId");
            DropColumn("dbo.MoneyTransactions", "WaitingGiverId");
        }
    }
}
