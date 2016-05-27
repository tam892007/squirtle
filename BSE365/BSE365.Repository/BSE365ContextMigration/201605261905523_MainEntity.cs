namespace BSE365.Repository.BSE365ContextMigration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MainEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        UserName = c.String(nullable: false, maxLength: 128),
                        State = c.Int(nullable: false),
                        Priority = c.Int(nullable: false),
                        LastCycleDate = c.DateTime(nullable: false),
                        UserInfoId = c.Int(nullable: false),
                        RelatedTransaction = c.String(),
                    })
                .PrimaryKey(t => t.UserName)
                .ForeignKey("dbo.UserInfoes", t => t.UserInfoId)
                .Index(t => t.UserInfoId);
            
            CreateTable(
                "dbo.MoneyTransactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GiverId = c.String(nullable: false, maxLength: 128),
                        ReceiverId = c.String(nullable: false, maxLength: 128),
                        Created = c.DateTime(nullable: false),
                        LastModified = c.DateTime(nullable: false),
                        State = c.Int(nullable: false),
                        AttachmentUrl = c.String(),
                        TransferedDate = c.DateTime(),
                        ReceivedDate = c.DateTime(),
                        MoneyTransferGroupId = c.Int(nullable: false),
                        IsEnd = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.GiverId)
                .ForeignKey("dbo.Accounts", t => t.ReceiverId)
                .Index(t => t.GiverId)
                .Index(t => t.ReceiverId);
            
            CreateTable(
                "dbo.WaitingGivers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountId = c.String(nullable: false, maxLength: 128),
                        Priority = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AccountId)
                .Index(t => t.AccountId);
            
            CreateTable(
                "dbo.WaitingReceivers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Amount = c.Int(nullable: false),
                        AccountId = c.String(nullable: false, maxLength: 128),
                        Priority = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AccountId)
                .Index(t => t.AccountId);
            
            CreateTable(
                "dbo.MoneyTransferGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Giver1Id = c.String(),
                        Giver2Id = c.String(),
                        Giver3Id = c.String(),
                        Receiver1Id = c.String(),
                        Receiver2Id = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WaitingReceivers", "AccountId", "dbo.Accounts");
            DropForeignKey("dbo.WaitingGivers", "AccountId", "dbo.Accounts");
            DropForeignKey("dbo.Accounts", "UserInfoId", "dbo.UserInfoes");
            DropForeignKey("dbo.MoneyTransactions", "ReceiverId", "dbo.Accounts");
            DropForeignKey("dbo.MoneyTransactions", "GiverId", "dbo.Accounts");
            DropIndex("dbo.WaitingReceivers", new[] { "AccountId" });
            DropIndex("dbo.WaitingGivers", new[] { "AccountId" });
            DropIndex("dbo.MoneyTransactions", new[] { "ReceiverId" });
            DropIndex("dbo.MoneyTransactions", new[] { "GiverId" });
            DropIndex("dbo.Accounts", new[] { "UserInfoId" });
            DropTable("dbo.MoneyTransferGroups");
            DropTable("dbo.WaitingReceivers");
            DropTable("dbo.WaitingGivers");
            DropTable("dbo.MoneyTransactions");
            DropTable("dbo.Accounts");
        }
    }
}
