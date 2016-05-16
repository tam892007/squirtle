using BSE365.Model.Entities;
using BSE365.Repository.DataContext;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE365.Repository.Repositories
{
    public class AuthRepository : IDisposable
    {
        private BSE365AuthContext _ctx;

        private UserManager<User> _userManager;

        public AuthRepository()
        {
            _ctx = new BSE365AuthContext();
            _userManager = new UserManager<User>(new UserStore<User>(_ctx));
        }

        public async Task<IdentityResult> RegisterUser(string userName, string password)
        {
            var user = new User
            {
                UserName = userName,
            };

            var result = await _userManager.CreateAsync(user, password);

            return result;
        }

        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            IdentityUser user = await _userManager.FindAsync(userName, password);
            return user;
        }

        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();

        }
    }
}
