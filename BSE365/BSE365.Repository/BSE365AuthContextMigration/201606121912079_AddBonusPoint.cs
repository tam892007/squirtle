namespace BSE365.Repository.BSE365AuthContextMigration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBonusPoint : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserInfoes", "BonusPoint", c => c.Int(nullable: false));
            AddColumn("dbo.UserInfoes", "TotalBonusPoint", c => c.Int(nullable: false));
            AddColumn("dbo.UserInfoes", "TotalGiveCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserInfoes", "TotalGiveCount");
            DropColumn("dbo.UserInfoes", "TotalBonusPoint");
            DropColumn("dbo.UserInfoes", "BonusPoint");
        }
    }
}
