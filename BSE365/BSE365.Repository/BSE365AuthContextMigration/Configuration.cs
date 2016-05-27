using BSE365.Base.Infrastructures;

namespace BSE365.Repository.BSE365AuthContextMigration
{
    using BSE365.Common.Constants;
    using BSE365.Model.Entities;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Data.Entity.Migrations;

    public sealed class Configuration : DbMigrationsConfiguration<BSE365.Repository.DataContext.BSE365AuthContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"BSE365AuthContextMigration";
            ContextKey = "BSE365.Repository.DataContext.BSE365AuthContext";
        }

        protected override void Seed(BSE365.Repository.DataContext.BSE365AuthContext context)
        {
            InitData(context);
        }

        public static void InitData(BSE365.Repository.DataContext.BSE365AuthContext context)
        {
            var _userManager = new UserManager<User>(new UserStore<User>(context));
            // admin
            var adminInfo = context.UserInfos.Add(new UserInfo
            {
                DisplayName = SystemAdmin.DisplayName,
                Email = SystemAdmin.Email,
                PhoneNumber = SystemAdmin.PhoneNumber,
                BankNumber = SystemAdmin.BankNumber,
                BankName = SystemAdmin.BankName,
                BankBranch = SystemAdmin.BankBranch,
            });
            context.SaveChanges();
            _userManager.Create(new User
            {
                UserName = SystemAdmin.UserName,
                UserInfo = adminInfo,
                PinBalance = int.MaxValue,
            }, SystemAdmin.Password);

            // test data
            for (int i = 0; i < TestData.UserNames.Length; i++)
            {
                var info = context.UserInfos.Add(new UserInfo
                {
                    ParentId = SystemAdmin.UserName,
                    DisplayName = TestData.DisplayNames[i],
                    Email = TestData.UserNames[i] + "@gmail.com",
                    PhoneNumber = TestData.PhoneNumbers[i],
                    BankNumber = TestData.BankNumbers[i],
                    BankName = TestData.BankNames[i],
                    BankBranch = TestData.BankBranchs[i],
                });
                context.SaveChanges();
                foreach (var suffix in TestData.AccountSuffixes)
                {
                    var username = TestData.UserNames[i] + suffix;
                    _userManager.Create(new User
                    {
                        UserName = username,
                        UserInfo = info,
                        PinBalance = 10,
                    }, TestData.Password);
                }
            }
            context.SaveChanges();
        }
    }
}