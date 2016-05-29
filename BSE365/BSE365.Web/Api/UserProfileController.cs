using BSE365.Mappings;
using BSE365.Model.Entities;
using BSE365.Repository.Repositories;
using BSE365.ViewModels;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
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
        [Route("GetCurrentUserContext")]
        public async Task<IHttpActionResult> GetCurrentUserContext()
        {
            var result = await GetCurrentUserContextAsync();
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

        [HttpPost]
        [Route("UpdateAvatar")]
        public async Task<IHttpActionResult> UpdateAvatar()
        {
            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);

            // extract file name and file contents
            var fileNameParam = provider.Contents[0].Headers.ContentDisposition.Parameters
                .FirstOrDefault(p => p.Name.ToLower() == "filename");
            string fileName = (fileNameParam == null) ? "" : fileNameParam.Value.Trim('"');
            byte[] file = await provider.Contents[0].ReadAsByteArrayAsync();

            var id = User.Identity.GetUserId();        
            var image = new Image
            {
                Content = file,
                Extension = fileName.Split('.').Last(),
            };

            var result = await _repo.UpdateAvatar(id, image);
            return Ok(result.ToViewModel());
        }

        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordViewModel vm)
        {
            var result = await ChangePasswordAsync(vm);
            return Ok(result);
        }

        [HttpGet] 
        [Route("CheckName")]
        public async Task<IHttpActionResult> CheckName(string name)
        {
            var result = await CheckNameAsync(name);
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

        private async Task<ResultViewModel<bool>> ChangePasswordAsync(ChangePasswordViewModel vm)
        {
            var id = User.Identity.GetUserId();
            var succeeded = await _repo.ChangePassword(id, vm.OldPassword, vm.NewPassword);
            return new ResultViewModel<bool>
            {
                IsSuccessful = succeeded,
                Result = succeeded,
            };
        }

        private async Task<ResultViewModel<bool>> CheckNameAsync(string name)
        {
            var result = new ResultViewModel<bool>();
            var user = await _repo.FindUserByName(name);
            result.IsSuccessful = user != null;
            result.Result = user != null;
            return result;
        }

        private async Task<UserContextViewModel> GetCurrentUserContextAsync()
        {
            var userId = User.Identity.GetUserId();
            var user = await _repo.FindUser(userId);
            var viewModel = user.ToContextViewModel();
            return viewModel;
        }
    }
}

