using BSE365.Common;
using BSE365.Model.Entities;
using BSE365.Repository.DataContext;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data;
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
                    dbTransaction.Commit();
                    result.IsSuccessful = identityResult.Succeeded;
                    result.Result = user;
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
