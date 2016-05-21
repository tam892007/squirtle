using BSE365.Base.Repositories;
using BSE365.Common.Constants;
using BSE365.Mappings;
using BSE365.Model.Entities;
using BSE365.Repository.Repositories;
using BSE365.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BSE365.Api
{
    [Authorize]
    [RoutePrefix("api/User")]
    public class UserProfileController : ApiController
    {
        private UserProfileRepository _repo = null;

        public UserProfileController()
        {
            _repo = new UserProfileRepository();
        }

        [HttpGet]
        [Route("GetCurrent")]
        public async Task<IHttpActionResult> GetCurrent()
        {
            var result = await GetCurrentUserProfile();
            return Ok(result);
        }

        [HttpGet]
        [Route("GetCurrentPin")]
        public async Task<IHttpActionResult> GetCurrentPin()
        {
            var result = await GetCurrentUserPinInfo();
            return Ok(result);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repo.Dispose();
            }

            base.Dispose(disposing);
        }

        private async Task<UserInfoViewModel> GetCurrentUserProfile()
        {            
            var userId = User.Identity.GetUserId();
            var user = await _repo.FindUser(userId);
            return user.ToViewModel();
        }

        private async Task<PinBalanceViewModel> GetCurrentUserPinInfo()
        {
            var userId = User.Identity.GetUserId();
            var user = await _repo.FindUser(userId);
            return user.ToPinBalanceViewModel();
        }
    }
}

