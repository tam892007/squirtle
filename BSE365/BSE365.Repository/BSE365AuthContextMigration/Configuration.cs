namespace BSE365.Repository.BSE365AuthContextMigration
{
    using BSE365.Common.Constants;
    using BSE365.Common.Helper;
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
            ////Add User Admin
            var numOfUser = context.UserInfos.Count();
            if (numOfUser == 0)
            {
                /////Add Role
                var _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                _roleManager.Create(new IdentityRole {Name = UserRolesText.SuperAdmin});
                _roleManager.Create(new IdentityRole {Name = UserRolesText.User});

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
                        Rating = 5,
                    },
                    PinBalance = 10000000,
                };

                _userManager.Create(user, SystemAdmin.Password);
                user.UserName = Utilities.StandardizeUserId(user.UserInfo.Id);
                _userManager.Update(user);
                _userManager.AddToRole(user.Id, UserRolesText.SuperAdmin);

                InitData(context);
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
        }

        public static void InitData(BSE365.Repository.DataContext.BSE365AuthContext context)
        {
            var _userManager = new UserManager<User>(new UserStore<User>(context));
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
                    var username = Utilities.StandardizeUserId(info.Id) + suffix;
                    var user = new User
                    {
                        UserName = username,
                        UserInfo = info,
                        PinBalance = 10,
                    };
                    _userManager.Create(user, TestData.Password);
                    //_userManager.AddToRole(user.Id, UserRolesText.User);
                }
            }
        }
    }
}
