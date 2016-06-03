namespace BSE365.Repository.BSE365ContextMigration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStoredProcedureMapGiversAndReceivers : DbMigration
    {
        public override void Up()
        {
            SqlFile("../../SQL/SpMapGiversAndReceivers.sql");
            SqlFile("../../SQL/SpUpdateNotConfirmedTransactions.sql");
            SqlFile("../../SQL/SpUpdateNotTransferedTransactions.sql");
            SqlFile("../../SQL/SpUpdateTransactions.sql");
        }
        
        public override void Down()
        {
            SqlFile("../../SQL/SpMapGiversAndReceivers_Drop.sql");
            SqlFile("../../SQL/SpUpdateTransaction_Drop.sql");
        }
    }
}
