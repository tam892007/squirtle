namespace BSE365.Repository.BSE365AuthContextMigration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWaitingType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserInfoes", "DayBonusTemp", c => c.DateTime(nullable: false));
            AddColumn("dbo.UserInfoes", "DayBonusPoint", c => c.Int(nullable: false));
            AddColumn("dbo.UserInfoes", "LastClaimBonusDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserInfoes", "LastClaimBonusDate");
            DropColumn("dbo.UserInfoes", "DayBonusPoint");
            DropColumn("dbo.UserInfoes", "DayBonusTemp");
        }
    }
}
