using BSE365.Common;
using BSE365.Model.Entities;
using BSE365.Repository.DataContext;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
            var user = await _userManager.FindByIdAsync(id);

            return user;             
        }

        public async Task<User> UpdateAvatar(string id, Image image)
        {
            var user = await FindUser(id);

            if (user.UserInfo.AvatarId.HasValue)
            {
                user.UserInfo.Avatar.Content = image.Content;
                user.UserInfo.Avatar.Extension = image.Extension;
            }
            else { 
                user.UserInfo.Avatar = new Image
                {
                    Content = image.Content,
                    Extension = image.Extension,
                };
            }

            await _userManager.UpdateAsync(user);

            return user;
        }

        public async Task<User> FindUserByName(string name)
        {
            var user = await _userManager.FindByNameAsync(name);

            return user;
        }

        public async Task<IEnumerable<User>> FindUserByUserInfo(int id)
        {
            var user = await _ctx.Users.Where(x => x.UserInfo.Id == id).OrderBy(x => x.UserName).ToListAsync();

            return user;
        }

        public async Task<bool> FindUserByBankNumber(string bankNumber, string userName)
        {
            bool exist;
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null || user.UserInfo == null)
            {
                exist = await _ctx.UserInfos.AnyAsync(x => x.BankNumber == bankNumber);
            }
            else
            {
                var id = user.UserInfo.Id;
                exist = await _ctx.UserInfos.AnyAsync(x => x.BankNumber == bankNumber && x.Id != id);
            }
            
            return exist;
        }

        public async Task<IEnumerable<User>> FindChildren(string id)
        {
            var users = await _ctx.Users.Where(x => x.UserInfo.ParentId == id).OrderBy(x => x.UserName).ToListAsync();

            return users;
        }

        public async Task<bool> ChangePassword(string id, string currentPassword, string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(id, currentPassword, newPassword);
            return result.Succeeded;            
        }

        public async Task<BusinessResult<User>> TransferPin(PinTransaction transaction)
        {
            var result = new BusinessResult<User>();
            IdentityResult giverResult = null;
            IdentityResult receiverResult = null;

            using (var dbTransaction = _ctx.Database.BeginTransaction(IsolationLevel.RepeatableRead))
            {
                try
                {
                    var user = await FindUser(transaction.FromId);
                    user.TransferPin(transaction.Amount);
                    giverResult = await _userManager.UpdateAsync(user);

                    var receiver = await FindUserByName(transaction.ToId);
                    receiver.ReceivePin(transaction.Amount);
                    receiverResult = await _userManager.UpdateAsync(receiver);

                    if (giverResult.Succeeded && receiverResult.Succeeded)
                    {
                        var history = new PinTransactionHistory
                        {
                            FromId = transaction.FromId,
                            FromName = transaction.FromName,
                            ToId = transaction.ToId,
                            Amount = transaction.Amount,
                            Code = transaction.Code,
                            Note = transaction.Note,
                            CreatedDate = DateTime.Now,
                        };

                        _ctx.PinTransactionHistories.Add(history);
                        await _ctx.SaveChangesAsync();                       

                        dbTransaction.Commit();

                        result.IsSuccessful = giverResult.Succeeded;
                        result.Result = user;
                    }
                    else
                    {
                        dbTransaction.Rollback();
                    }                 
                }
                catch (Exception ex)
                {
                    dbTransaction.Rollback();
                }
            }

            return result;
        }

        public async Task<BusinessResult<User>> UpdateUserInfo(string userId, UserInfo info)
        {
            var user = await FindUser(userId);
            
            ////Only update legal information
            user.UserInfo.DisplayName = info.DisplayName;
            user.UserInfo.PhoneNumber = info.PhoneNumber;
            user.UserInfo.Email = info.Email;
            user.UserInfo.BankBranch = info.BankBranch;
            user.UserInfo.BankNumber = info.BankNumber;
            user.UserInfo.BankName = info.BankName;

            var identityResult = await _userManager.UpdateAsync(user);

            var result = new BusinessResult<User>();
            result.IsSuccessful = identityResult.Succeeded;
            result.Result = user;

            return result;
        }

        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();
        }
    }
}
