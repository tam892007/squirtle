namespace BSE365.Repository.BSE365ContextMigration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnAvatarAndImageTable : DbMigration
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
            
        }
        
        public override void Down()
        {
            //DropTable("dbo.Images");
        }
    }
}
