namespace BSE365.Repository.BSE365ContextMigration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMessage : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        FromId = c.String(nullable: false, maxLength: 128),
                        ToId = c.String(nullable: false, maxLength: 128),
                        Status = c.Int(nullable: false),
                        SendTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.FromId)
                .ForeignKey("dbo.Accounts", t => t.ToId)
                .Index(t => t.FromId)
                .Index(t => t.ToId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "ToId", "dbo.Accounts");
            DropForeignKey("dbo.Messages", "FromId", "dbo.Accounts");
            DropIndex("dbo.Messages", new[] { "ToId" });
            DropIndex("dbo.Messages", new[] { "FromId" });
            DropTable("dbo.Messages");
        }
    }
}
