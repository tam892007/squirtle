using BSE365.Common;
using BSE365.Common.Helper;
using BSE365.Model.Entities;
using BSE365.Repository.DataContext;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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

        public async Task<BusinessResult<IEnumerable<User>>> RegisterUser(string userName, string password, UserInfo info)
        {
            var userInfo = info;      
            _ctx.UserInfos.Add(userInfo);
            await _ctx.SaveChangesAsync();

            var lstUser = new List<User>();
            var lstName = Utilities.GetRangeUserName(userInfo.Id);
            foreach (var name in lstName)
            {
                var user = new User
                {
                    UserName = name,
                    UserInfo = info,
                };

                await _userManager.CreateAsync(user, password);

                lstUser.Add(user);
            }            

            var result = new BusinessResult<IEnumerable<User>>();
            result.Result = lstUser;
            result.IsSuccessful = true;

            return result;
        }

        public async Task<User> FindUser(string userName, string password)
        {
            User user = await _userManager.FindAsync(userName, password);

            return user;
        }

        public Client FindClient(string clientId)
        {
            var client = _ctx.Clients.Find(clientId);

            return client;
        }

        public async Task<bool> AddRefreshToken(RefreshToken token)
        {

            var existingToken = _ctx.RefreshTokens.Where(r => r.Subject == token.Subject && r.ClientId == token.ClientId).SingleOrDefault();

            if (existingToken != null)
            {
                var result = await RemoveRefreshToken(existingToken);
            }

            _ctx.RefreshTokens.Add(token);

            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
            var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);

            if (refreshToken != null)
            {
                _ctx.RefreshTokens.Remove(refreshToken);
                return await _ctx.SaveChangesAsync() > 0;
            }

            return false;
        }

        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            _ctx.RefreshTokens.Remove(refreshToken);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);

            return refreshToken;
        }

        public List<RefreshToken> GetAllRefreshTokens()
        {
            return _ctx.RefreshTokens.ToList();
        }

        public async Task<User> FindAsync(UserLoginInfo loginInfo)
        {
            User user = await _userManager.FindAsync(loginInfo);

            return user;
        }

        public async Task<IdentityResult> CreateAsync(User user)
        {
            var result = await _userManager.CreateAsync(user);

            return result;
        }

        public async Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        {
            var result = await _userManager.AddLoginAsync(userId, login);

            return result;
        }

        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();

        }
    }
}
