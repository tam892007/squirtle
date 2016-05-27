using BSE365.Repository.DataContext;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace BSE365.Web
{
    public partial class Startup
    {
        public static void DatabaseInitializer()
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<BSE365AuthContext>());
            Database.SetInitializer(new MigrateDatabaseToLatestVersion
                <BSE365Context, BSE365.Repository.BSE365ContextMigration.Configuration>());
        }
    }
}