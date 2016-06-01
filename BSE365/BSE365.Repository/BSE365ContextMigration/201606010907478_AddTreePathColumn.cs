namespace BSE365.Repository.BSE365ContextMigration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTreePathColumn : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.UserInfoes", "TreePath", c => c.String());
        }
        
        public override void Down()
        {
            //DropColumn("dbo.UserInfoes", "TreePath");
        }
    }
}
