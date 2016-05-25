namespace BSE365.Repository.BSE365AuthContextMigration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LazyloadImageById : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserInfoes", "AvatarId", "dbo.Images");
            DropIndex("dbo.UserInfoes", new[] { "AvatarId" });
            AlterColumn("dbo.UserInfoes", "AvatarId", c => c.Int());
            CreateIndex("dbo.UserInfoes", "AvatarId");
            AddForeignKey("dbo.UserInfoes", "AvatarId", "dbo.Images", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserInfoes", "AvatarId", "dbo.Images");
            DropIndex("dbo.UserInfoes", new[] { "AvatarId" });
            AlterColumn("dbo.UserInfoes", "AvatarId", c => c.Int(nullable: false));
            CreateIndex("dbo.UserInfoes", "AvatarId");
            AddForeignKey("dbo.UserInfoes", "AvatarId", "dbo.Images", "Id", cascadeDelete: true);
        }
    }
}
