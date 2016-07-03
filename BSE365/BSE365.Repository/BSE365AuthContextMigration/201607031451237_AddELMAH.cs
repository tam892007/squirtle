namespace BSE365.Repository.BSE365AuthContextMigration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddELMAH : DbMigration
    {
        public override void Up()
        {
            SqlFile("SQL/ELMAH.db.SQLServer.sql");
        }
        
        public override void Down()
        {
            DropTable("ELMAH_Error");
            Sql("DROP PROCEDURE ELMAH_GetErrorXml");
            Sql("DROP PROCEDURE ELMAH_GetErrorsXml");
            Sql("DROP PROCEDURE ELMAH_LogError");
        }
    }
}
