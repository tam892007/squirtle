namespace BSE365.Repository.BSE365AuthContextMigration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserPrefix : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserInfoes", "UserPrefix", c => c.String());
            AddColumn("dbo.UserInfoes", "Created", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserInfoes", "Created");
            DropColumn("dbo.UserInfoes", "UserPrefix");
        }
    }
}
