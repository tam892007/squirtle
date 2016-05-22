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
            User user = await _userManager.FindByIdAsync(id);

            return user;             
        }

        public async Task<IEnumerable<User>> FindChildren(string id)
        {
            var users = await _ctx.Users.Where(x => x.UserInfo.ParentId == id).ToListAsync();

            return users;
        }

        public async Task<BusinessResult<User>> TransferPin(PinTransaction transaction)
        {
            var result = new BusinessResult<User>();
            IdentityResult identityResult = null;
            using (var dbTransaction = _ctx.Database.BeginTransaction(IsolationLevel.RepeatableRead))
            {
                try
                {
                    var user = await FindUser(transaction.FromId);
                    user.TransferPin(transaction.Amount);
                    identityResult = await _userManager.UpdateAsync(user);

                    if (identityResult.Succeeded)
                    {
                        var history = new PinTransactionHistory
                        {
                            FromId = transaction.FromId,
                            ToId = transaction.ToId,
                            Amount = transaction.Amount,
                            Code = transaction.Code,
                            Note = transaction.Note,
                            CreatedDate = DateTime.Now,
                        };

                        _ctx.PinTransactionHistories.Add(history);
                        await _ctx.SaveChangesAsync();                       

                        dbTransaction.Commit();

                        result.IsSuccessful = identityResult.Succeeded;
                        result.Result = user;
                    }                                       
                }
                catch (Exception ex)
                {
                    dbTransaction.Rollback();
                }
            }

            return result;
        }

        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();
        }
    }
}
