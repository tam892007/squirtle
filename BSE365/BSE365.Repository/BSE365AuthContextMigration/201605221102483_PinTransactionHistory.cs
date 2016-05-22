namespace BSE365.Repository.BSE365AuthContextMigration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PinTransactionHistory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PinTransactionHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedDate = c.DateTime(nullable: false),
                        FromId = c.String(),
                        ToId = c.String(),
                        Amount = c.Int(nullable: false),
                        Note = c.String(),
                        Code = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PinTransactionHistories");
        }
    }
}
