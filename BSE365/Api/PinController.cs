using BSE365.Common;
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
    [RoutePrefix("api/pin")]
    public class PinController : ApiController
    {
        private UserProfileRepository _repo = null;

        public PinController()
        {
            _repo = new UserProfileRepository();
        }

        [HttpPost]
        [Route("transfer")]
        public async Task<IHttpActionResult> Transfer(PinTransactionViewModel transactionVM)
        {
            var result = await TransferAsync(transactionVM);
            if (result.IsSuccessful)
            {
                return Ok(result.Result);
            }               
            else
            {
                return BadRequest();
            }
        }

        private async Task<ResultViewModel<PinBalanceViewModel>> TransferAsync(PinTransactionViewModel transactionVM)
        {
            var pinTransaction = transactionVM.ToModel();
            
            ////ensure from current user
            pinTransaction.FromId = User.Identity.GetUserId();

            var result = await _repo.TransferPin(pinTransaction);

            return result.ToResultViewModel<PinBalanceViewModel, User>((x) => { return x.ToPinBalanceViewModel(); });
        } 
    }
}

