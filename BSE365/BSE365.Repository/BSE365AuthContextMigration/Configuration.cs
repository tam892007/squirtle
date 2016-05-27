using BSE365.Base.Infrastructures;

namespace BSE365.Repository.BSE365AuthContextMigration
{
    using BSE365.Common.Constants;
    using BSE365.Model.Entities;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Data.Entity.Migrations;
    using System.Linq;

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
<<<<<<< HEAD
            ////Add User Admin
            var numOfUser = context.UserInfos.Count();
            if (numOfUser == 0)
            {
                var _userManager = new UserManager<User>(new UserStore<User>(context));
                User user = new User
                {
                    UserName = SystemAdmin.UserName,
                    UserInfo = new UserInfo
                    {
                        DisplayName = SystemAdmin.DisplayName,
                        Email = SystemAdmin.Email,
                        PhoneNumber = SystemAdmin.PhoneNumber,
                        BankNumber = SystemAdmin.BankNumber,
                        BankName = SystemAdmin.BankName,
                        BankBranch = SystemAdmin.BankBranch,
                    },
                    PinBalance = int.MaxValue,
                };

                _userManager.Create(user, SystemAdmin.Password);

                user.UserName = Utilities.StandardizeUserId(user.UserInfo.Id);

                _userManager.Update(user);
            }

            ////Add Client
            var client = context.Clients.Find(SystemAdmin.ClientId);
            if (client == null)
            {
                client = new Client
                {
                    Id = SystemAdmin.ClientId,
                    Name = SystemAdmin.ClientId,
                    ApplicationType = 0,
                    AllowedOrigin = "*",
                    Active = true,
                    RefreshTokenLifeTime = 36000,
                    Secret = SystemAdmin.ClientSecret,
                };

                context.Clients.Add(client);
                context.SaveChanges();
            }
=======
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
                UserName = adminInfo.Id.ToString(),
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
                    var username = info.Id.ToString() + suffix;
                    _userManager.Create(new User
                    {
                        UserName = username,
                        UserInfo = info,
                        PinBalance = 10,
                    }, TestData.Password);
                }
            }
            context.SaveChanges();
>>>>>>> e67c157c622d24c62947cd2790ca67193ffe3704
        }
    }
}
