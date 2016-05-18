using BSE365.Model.Entities;
using BSE365.Repository.DataContext;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSE365.Repository.Repositories
{
    public class UserProfileRepository : IDisposable
    {
        private BSE365AuthContext _ctx;

        private UserManager<User> _userManager;

        public UserProfileRepository()
        {
            _ctx = new BSE365AuthContext();
            _userManager = new UserManager<User>(new UserStore<User>(_ctx));
        }

        public async Task<User> FindUser(string id)
        {
            User user = await _userManager.FindByIdAsync(id);

            return user;             
        }

        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();
        }
    }
}
