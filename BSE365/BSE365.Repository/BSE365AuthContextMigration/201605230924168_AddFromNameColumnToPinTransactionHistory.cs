namespace BSE365.Repository.BSE365AuthContextMigration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFromNameColumnToPinTransactionHistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PinTransactionHistories", "FromName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PinTransactionHistories", "FromName");
        }
    }
}
