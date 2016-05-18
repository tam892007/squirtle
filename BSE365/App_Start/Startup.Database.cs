using BSE365.Repository.DataContext;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace BSE365.Web
{
    public partial class Startup
    {
        public static void DatabaseInitializer()
        {
            Database.SetInitializer(new DataInitializer());
        }

        public class DataInitializer : DropCreateDatabaseIfModelChanges<BSE365AuthContext>
        {
            protected override void Seed(BSE365AuthContext context)
            {
                try
                {
                    //seed data here                          
                }
                catch (DbEntityValidationException e)
                {
                    throw;
                }

                base.Seed(context);
            }
        }
    }
}