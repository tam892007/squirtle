using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BSE365.Common;
using BSE365.Common.Constants;
using BSE365.Common.Helper;
using BSE365.Model.Entities;
using BSE365.Repository.DataContext;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;

namespace BSE365.Repository.Repositories
{
    public class AuthRepository : IDisposable
    {
        private readonly BSE365AuthContext _ctx;

        private readonly UserManager<User> _userManager;

        public AuthRepository()
        {
            _ctx = new BSE365AuthContext();
            _userManager = new UserManager<User>(new UserStore<User>(_ctx));
        }

        public AuthRepository(IDataProtectionProvider provider) : this()
        {
            _userManager.UserTokenProvider = new DataProtectorTokenProvider<User>(provider.Create("ResetPassword"));
        }

        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();
        }

        public async Task<BusinessResult<IEnumerable<User>>> RegisterUser(string userName, string password,
            UserInfo info)
        {
            var parentUser = _userManager.FindById(info.ParentId);
            if (parentUser == null)
                throw new Exception(string.Format("Parent User with id {0} doesnt exist", info.ParentId));
            var parentLevel = parentUser.UserInfo.Level;
            var parentPath = parentUser.UserInfo.TreePath;

            var userInfo = info;
            userInfo.Level = parentLevel + 1; ////Update level for tree
            userInfo.TreePath = string.Format("{0}{1}{2}", parentPath, SystemAdmin.TreePathSplitter, parentUser.Id);
            _ctx.UserInfos.Add(userInfo);
            await _ctx.SaveChangesAsync();

            var lstUser = new List<User>();
            var lstName = Utilities.GetRangeUserName(userInfo.Id);
            foreach (var name in lstName)
            {
                var user = new User
                {
                    UserName = name,
                    UserInfo = info
                };

                await _userManager.CreateAsync(user, password);

                lstUser.Add(user);

                ////create account
                var account = new Account
                {
                    UserName = name,
                    UserInfoId = info.Id
                };

                _ctx.Accounts.Add(account);
            }

            await _ctx.SaveChangesAsync();

            var result = new BusinessResult<IEnumerable<User>>();
            result.Result = lstUser;
            result.IsSuccessful = true;

            return result;
        }

        public async Task<User> FindUser(string userName, string password)
        {
            var user = await _userManager.FindAsync(userName, password);

            return user;
        }

        public async Task<User> FindUserByName(string name)
        {
            var user = await _userManager.FindByNameAsync(name);

            return user;
        }

        public Client FindClient(string clientId)
        {
            var client = _ctx.Clients.Find(clientId);

            return client;
        }

        public async Task<bool> AddRefreshToken(RefreshToken token)
        {
            var existingTokens =
                _ctx.RefreshTokens.Where(r => r.Subject == token.Subject && r.ClientId == token.ClientId);

            if (existingTokens != null && existingTokens.Count() > 0)
            {
                var result = await RemoveRangeRefreshToken(existingTokens);
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

        public async Task<bool> RemoveRangeRefreshToken(IEnumerable<RefreshToken> refreshTokens)
        {
            _ctx.RefreshTokens.RemoveRange(refreshTokens);
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
            var user = await _userManager.FindAsync(loginInfo);

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

        public async Task<string> ForgotPassword(User user)
        {
            if (user == null) return string.Empty;

            var code = await _userManager.GeneratePasswordResetTokenAsync(user.Id);

            return code;
        }

        public async Task<IdentityResult> ResetPassword(User user, string code, string newPassword)
        {
            if (user == null) return null;
            var result = await _userManager.ResetPasswordAsync(user.Id, code, newPassword);
            return result;
        }

        public async Task<IdentityResult> ForceResetPassword(User user)
        {
            if (user == null) return null;
            var code = await _userManager.GeneratePasswordResetTokenAsync(user.Id);
            var result = await _userManager.ResetPasswordAsync(user.Id, code, SystemAdmin.DefaultPassword);
            return result;
        }

        public async Task<bool> ForceChangeUserPassword(string userPrefix, string password)
        {
            var usernames = Utilities.GetRangeUserName(userPrefix);
            var users = await _ctx.Users.Where(x => usernames.Contains(x.UserName)).ToListAsync();
            if (users.Count == 0) return false;
            foreach (var user in users)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user.Id);
                var flag = await _userManager.ResetPasswordAsync(user.Id, token, password);
            }
            return true;
        }

        public async Task<IList<string>> GetRolesForUser(string userId)
        {
            return await _userManager.GetRolesAsync(userId);
        }
    }
}