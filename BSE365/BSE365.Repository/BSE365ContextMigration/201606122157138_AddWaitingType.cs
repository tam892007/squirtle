namespace BSE365.Repository.BSE365ContextMigration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWaitingType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WaitingGivers", "Type", c => c.Int(nullable: false));
            AddColumn("dbo.WaitingReceivers", "Type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WaitingReceivers", "Type");
            DropColumn("dbo.WaitingGivers", "Type");
        }
    }
}
