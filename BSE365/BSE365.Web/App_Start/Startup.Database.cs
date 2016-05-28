using BSE365.Repository.DataContext;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace BSE365.Web
{
    public partial class Startup
    {
        public static void DatabaseInitializer()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<BSE365AuthContext, BSE365.Repository.BSE365AuthContextMigration.Configuration>());
            Database.SetInitializer(new MigrateDatabaseToLatestVersion
                <BSE365Context, BSE365.Repository.BSE365ContextMigration.Configuration>());

            var configuration = new BSE365.Repository.BSE365AuthContextMigration.Configuration();
            var migrator = new System.Data.Entity.Migrations.DbMigrator(configuration);
            if (migrator.GetPendingMigrations().Any())
            {
                migrator.Update();
            }

            var configuration2 = new BSE365.Repository.BSE365ContextMigration.Configuration();
            var migrator2 = new System.Data.Entity.Migrations.DbMigrator(configuration);
            if (migrator2.GetPendingMigrations().Any())
            {
                migrator2.Update();
            }
        }
    }
}