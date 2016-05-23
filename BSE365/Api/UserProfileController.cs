using BSE365.Common;
using BSE365.Mappings;
using BSE365.Model.Entities;
using BSE365.Repository.Repositories;
using BSE365.ViewModels;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
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

        [HttpPost]
        [Route("UpdateCurrent")]
        public async Task<IHttpActionResult> UpdateCurrent(UserInfoViewModel viewModel)
        {
            var result = await UpdateCurrentAsync(viewModel);
            if (result.IsSuccessful)
            {
                return Ok(result.Result);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("GetChildren")]
        public async Task<IHttpActionResult> GetChildren(string id)
        {
            var result = await GetChildrenAsync(id);
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
            var viewModel = user.ToViewModel();

            if (user.UserInfo != null && !string.IsNullOrEmpty(user.UserInfo.ParentId))
            {
                var parentUser = await _repo.FindUser(user.UserInfo.ParentId);
                viewModel.ParentName = parentUser.UserName;
            }
            
            return viewModel;
        }

        private async Task<IEnumerable<UserInfoViewModel>> GetChildrenAsync(string id)
        {
            var result = await _repo.FindChildren(id);
            return result.Select(x => x.ToViewModel());
        }

        private async Task<PinBalanceViewModel> GetCurrentUserPinInfo()
        {
            var userId = User.Identity.GetUserId();
            var user = await _repo.FindUser(userId);
            return user.ToPinBalanceViewModel();
        }

        private async Task<ResultViewModel<UserInfoViewModel>> UpdateCurrentAsync(UserInfoViewModel vm)
        {            
            var userId = User.Identity.GetUserId();
            var result = await _repo.UpdateUserInfo(userId, vm.ToModel());
            return result.ToResultViewModel<UserInfoViewModel, User>(x => x.ToViewModel());
        }
    }
}

