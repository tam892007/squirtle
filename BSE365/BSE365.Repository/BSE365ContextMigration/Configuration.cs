using BSE365.Base.Infrastructures;
using BSE365.Common.Constants;
using BSE365.Model.Entities;

namespace BSE365.Repository.BSE365ContextMigration
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<BSE365.Repository.DataContext.BSE365Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"BSE365ContextMigration";
            ContextKey = "BSE365.Repository.DataContext.BSE365Context";
        }

        protected override void Seed(BSE365.Repository.DataContext.BSE365Context context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            InitData(context);
        }

        public static void InitData(BSE365.Repository.DataContext.BSE365Context context)
        {
            for (int i = 0; i < TestData.UserNames.Length; i++)
            {
                var banknumber = TestData.BankNumbers[i];
                var info = context.UserInfos.First(x => x.BankNumber == banknumber);
                foreach (var suffix in TestData.AccountSuffixes)
                {
                    var username = TestData.UserNames[i] + suffix;
                    var account = new Account
                    {
                        UserName = username,
                        UserInfoId = info.Id,
                        ObjectState = ObjectState.Added,
                    };
                    if (i%2 != 0)
                    {
                        account.WaitingReceivers.Add(new WaitingReceiver
                        {
                            Created = DateTime.Now,
                            ObjectState = ObjectState.Added,
                        });
                    }
                    else
                    {
                        account.WaitingGivers.Add(new WaitingGiver
                        {
                            Created = DateTime.Now,
                            ObjectState = ObjectState.Added,
                        });
                    }
                    context.Accounts.Add(account);
                }
            }
        }
    }
}