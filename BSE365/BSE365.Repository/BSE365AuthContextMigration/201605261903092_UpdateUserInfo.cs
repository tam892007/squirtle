namespace BSE365.Repository.BSE365AuthContextMigration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateUserInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserInfoes", "LastGiveDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.UserInfoes", "GiveOver", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserInfoes", "GiveOver");
            DropColumn("dbo.UserInfoes", "LastGiveDate");
        }
    }
}
