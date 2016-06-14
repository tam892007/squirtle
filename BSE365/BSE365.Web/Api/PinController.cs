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
using BSE365.Helper;

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
            var response = CatpchaValidator.Validate(transactionVM.Code);
            if (!response.Success) return BadRequest("invalid_captcha");

            var result = await TransferAsync(transactionVM);
            if (result.IsSuccessful)
            {
                return Ok(result.Result);
            }               
            else
            {
                return BadRequest("err_server");
            }
        }

        [Route("GetAll")]
        [HttpPost]
        public async Task<IHttpActionResult> GetAll(FilterVM filter)
        {
            var result = await GetAllAsync(filter);
            return Ok(result);
        }

        private async Task<PageViewModel<PinTransactionViewModel>> GetAllAsync(FilterVM filter)
        {
            int totalPageCount;
            var userId = User.Identity.GetUserName();
            var transactionHistories = await _historyRepo.Query(x=>x.FromName == userId || x.ToId == userId)
                .OrderBy(x => x.OrderByDescending(i => i.CreatedDate))
                .SelectPage(filter.Pagination.Start / filter.Pagination.Number + 1, filter.Pagination.Number,
                            out totalPageCount).ToListAsync();

            var data = transactionHistories.Select(x => x.ToViewModel(userId));

            var page = new PageViewModel<PinTransactionViewModel>
            {
                Data = data,
                TotalItems = totalPageCount
            };

            return page;
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

