namespace BSE365.Repository.BSE365ContextMigration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAvatar : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.Images",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            Extension = c.String(),
            //            Content = c.Binary(),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //AddColumn("dbo.PinTransactionHistories", "FromName", c => c.String());
        }
        
        public override void Down()
        {
            //DropColumn("dbo.PinTransactionHistories", "FromName");
            //DropTable("dbo.Images");
        }
    }
}
