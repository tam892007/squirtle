namespace BSE365.Repository.BSE365ContextMigration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateWaitingTransactionUserInfoAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "CurrentTransactionGroupId", c => c.Int());
            AddColumn("dbo.WaitingGivers", "Amount", c => c.Int(nullable: false));
            AddColumn("dbo.MoneyTransactions", "Type", c => c.Int(nullable: false));
            AddColumn("dbo.MoneyTransactions", "RelatedTransactionId", c => c.Int());
            CreateIndex("dbo.MoneyTransactions", "RelatedTransactionId");
            AddForeignKey("dbo.MoneyTransactions", "RelatedTransactionId", "dbo.MoneyTransactions", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MoneyTransactions", "RelatedTransactionId", "dbo.MoneyTransactions");
            DropIndex("dbo.MoneyTransactions", new[] { "RelatedTransactionId" });
            DropColumn("dbo.MoneyTransactions", "RelatedTransactionId");
            DropColumn("dbo.MoneyTransactions", "Type");
            DropColumn("dbo.WaitingGivers", "Amount");
            DropColumn("dbo.Accounts", "CurrentTransactionGroupId");
        }
    }
}
