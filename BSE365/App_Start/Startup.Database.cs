using BSE365.Repository.DataContext;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;

namespace BSE365.Web
{
    public partial class Startup
    {
        public static void DatabaseInitializer()
        {            
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<BSE365AuthContext, BSE365.Repository.BSE365AuthContextMigration.Configuration>());
            var dbMigrator = new DbMigrator(new BSE365.Repository.BSE365AuthContextMigration.Configuration());
            dbMigrator.Update();

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<BSE365Context, BSE365.Repository.BSE365ContextMigration.Configuration>());
            dbMigrator = new DbMigrator(new BSE365.Repository.BSE365ContextMigration.Configuration());
            dbMigrator.Update();
        }
    }
}