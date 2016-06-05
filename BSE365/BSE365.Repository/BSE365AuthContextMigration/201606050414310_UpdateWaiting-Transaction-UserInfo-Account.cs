namespace BSE365.Repository.BSE365AuthContextMigration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateWaitingTransactionUserInfoAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserInfoes", "State", c => c.Int(nullable: false));
            AddColumn("dbo.UserInfoes", "IsAllowAbandonOne", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserInfoes", "RelatedAccount", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserInfoes", "RelatedAccount");
            DropColumn("dbo.UserInfoes", "IsAllowAbandonOne");
            DropColumn("dbo.UserInfoes", "State");
        }
    }
}
