namespace BSE365.Repository.BSE365AuthContextMigration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SupportTreeAndRating : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserInfoes", "Rating", c => c.Int(nullable: false));
            AddColumn("dbo.UserInfoes", "Level", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserInfoes", "Level");
            DropColumn("dbo.UserInfoes", "Rating");
        }
    }
}
