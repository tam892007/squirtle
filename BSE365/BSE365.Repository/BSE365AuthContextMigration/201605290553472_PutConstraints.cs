namespace BSE365.Repository.BSE365AuthContextMigration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PutConstraints : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserInfoes", "DisplayName", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.UserInfoes", "Email", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.UserInfoes", "PhoneNumber", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.UserInfoes", "BankNumber", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.UserInfoes", "BankName", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.UserInfoes", "BankBranch", c => c.String(nullable: false, maxLength: 30));
            CreateIndex("dbo.UserInfoes", "BankNumber", unique: true, name: "Unq_BankNumber");
        }
        
        public override void Down()
        {
            DropIndex("dbo.UserInfoes", "Unq_BankNumber");
            AlterColumn("dbo.UserInfoes", "BankBranch", c => c.String(nullable: false));
            AlterColumn("dbo.UserInfoes", "BankName", c => c.String(nullable: false));
            AlterColumn("dbo.UserInfoes", "BankNumber", c => c.String(nullable: false));
            AlterColumn("dbo.UserInfoes", "PhoneNumber", c => c.String(nullable: false));
            AlterColumn("dbo.UserInfoes", "Email", c => c.String(nullable: false));
            AlterColumn("dbo.UserInfoes", "DisplayName", c => c.String(nullable: false));
        }
    }
}
