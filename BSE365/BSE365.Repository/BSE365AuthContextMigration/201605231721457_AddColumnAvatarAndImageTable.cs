namespace BSE365.Repository.BSE365AuthContextMigration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnAvatarAndImageTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Images",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Extension = c.String(),
                        Content = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.UserInfoes", "Avatar_Id", c => c.Int());
            CreateIndex("dbo.UserInfoes", "Avatar_Id");
            AddForeignKey("dbo.UserInfoes", "Avatar_Id", "dbo.Images", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserInfoes", "Avatar_Id", "dbo.Images");
            DropIndex("dbo.UserInfoes", new[] { "Avatar_Id" });
            DropColumn("dbo.UserInfoes", "Avatar_Id");
            DropTable("dbo.Images");
        }
    }
}
