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
        }
    }
}
