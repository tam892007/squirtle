using BSE365.Base.Repositories.Contracts;
using BSE365.Mappings;
using BSE365.Model.Entities;
using BSE365.Repository.Repositories;
using BSE365.ViewModels;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace BSE365.Api
{
    [Authorize]
    [RoutePrefix("api/pin")]
    public class PinController : ApiController
    {
        private UserProfileRepository _repo = null;
        IRepositoryAsync<PinTransactionHistory> _historyRepo;

        public PinController(IRepositoryAsync<PinTransactionHistory> historyRepo)
        {
            _repo = new UserProfileRepository();
            _historyRepo = historyRepo;
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

        [Route("GetAll")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAll()
        {
            var result = await GetAllAsync();
            return Ok(result);
        }

        private async Task<IEnumerable<PinTransactionViewModel>> GetAllAsync()
        {
            var userId = User.Identity.GetUserId();
            var transactionHistories = await _historyRepo.Query(x=>x.FromId == userId)
                .OrderBy(x=>x.OrderByDescending(i=>i.CreatedDate)).SelectAsync();
            return transactionHistories.Select(x => x.ToViewModel());
        }

        private async Task<ResultViewModel<PinBalanceViewModel>> TransferAsync(PinTransactionViewModel transactionVM)
        {
            var pinTransaction = transactionVM.ToModel();
            
            ////ensure from current user
            pinTransaction.FromId = User.Identity.GetUserId();
            pinTransaction.FromName = User.Identity.GetUserName();

            var result = await _repo.TransferPin(pinTransaction);

            return result.ToResultViewModel<PinBalanceViewModel, User>((x) => { return x.ToPinBalanceViewModel(); });
        } 
    }
}

