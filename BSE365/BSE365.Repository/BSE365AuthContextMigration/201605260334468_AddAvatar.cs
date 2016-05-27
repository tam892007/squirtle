namespace BSE365.Repository.BSE365AuthContextMigration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAvatar : DbMigration
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
            
            AddColumn("dbo.UserInfoes", "AvatarId", c => c.Int());
            CreateIndex("dbo.UserInfoes", "AvatarId");
            AddForeignKey("dbo.UserInfoes", "AvatarId", "dbo.Images", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserInfoes", "AvatarId", "dbo.Images");
            DropIndex("dbo.UserInfoes", new[] { "AvatarId" });
            DropColumn("dbo.UserInfoes", "AvatarId");
            DropTable("dbo.Images");
        }
    }
}
