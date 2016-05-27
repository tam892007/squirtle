using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using BSE365.Base.Repositories.Contracts;
using BSE365.Base.UnitOfWork;
using BSE365.Base.UnitOfWork.Contracts;
using BSE365.Mappings;
using BSE365.Model.Entities;
using BSE365.Repository.Repositories;
using BSE365.ViewModels;
using Microsoft.AspNet.Identity;

namespace BSE365.Api
{
    //[Authorize]
    [RoutePrefix("api/transaction")]
    public class MoneyTransactionController : ApiController
    {
        IUnitOfWorkAsync _unitOfWork;
        IRepositoryAsync<MoneyTransaction> _transactionRepo;

        #region

        #endregion

        public MoneyTransactionController(
            IUnitOfWorkAsync unitOfWork,
            IRepositoryAsync<MoneyTransaction> transactionRepo)
        {
            _unitOfWork = unitOfWork;
            _transactionRepo = transactionRepo;
        }

        /// <summary>
        /// danh sách những người mình sẽ phải cho tại phiên hiện tại
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GiveRequested")]
        public async Task<IHttpActionResult> GiveRequested()
        {
            var result = await GiveRequestedAsync();
            return Ok(result);
        }

        /// <summary>
        /// danh sách những người sẽ cho mình tại phiên hiện tại
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("ReceiveRequested")]
        public async Task<IHttpActionResult> ReceiveRequested()
        {
            var result = await ReceiveRequestedAsync();
            return Ok(result);
        }

        /// <summary>
        /// xác nhận đã gửi
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("MoneyTranfered")]
        public async Task<IHttpActionResult> MoneyTranfered(int transactionId)
        {
            var fileUrl = "";
            await MoneyTranferedAsync(transactionId, fileUrl);
            return Ok();
        }

        /// <summary>
        /// xác nhận đã nhận
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("MoneyReceived")]
        public async Task<IHttpActionResult> MoneyReceived(int transactionId)
        {
            await MoneyReceivedAsync(transactionId);
            return Ok();
        }

        /// <summary>
        /// report việc không gửi nhưng lại xác nhận đã gửi trên hệ thống
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ReportNotTransfer")]
        public async Task<IHttpActionResult> ReportNotTransfer(int transactionId)
        {
            await ReportNotTransferAsync(transactionId);
            return Ok();
        }

        #region internal method

        private async Task<List<MoneyTransactionVM.Receiver>> GiveRequestedAsync()
        {
            var expression = MoneyTransactionVMMapping.GetExpToReceiverVM();
            var data = await _transactionRepo.Queryable().Where(x =>
                !x.IsEnd
                && x.ReceiverId == User.Identity.GetUserName())
                .Include(x => x.Receiver.UserInfo)
                .Select(expression)
                .ToListAsync();
            return data;
        }

        private async Task<List<MoneyTransactionVM.Giver>> ReceiveRequestedAsync()
        {
            var expression = MoneyTransactionVMMapping.GetExpToGiverVM();
            var data = await _transactionRepo.Queryable().Where(x =>
                !x.IsEnd
                && x.GiverId == User.Identity.GetUserName())
                .Include(x => x.Giver.UserInfo)
                .Select(expression)
                .ToListAsync();
            return data;
        }

        private async Task MoneyTranferedAsync(int transactionId, string fileUrl)
        {
            var transaction = await _transactionRepo.FindAsync(transactionId);
            transaction.MoneyTranfered(fileUrl);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task MoneyReceivedAsync(int transactionId)
        {
            var transaction = await _transactionRepo.FindAsync(transactionId);
            transaction.MoneyReceived();
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task ReportNotTransferAsync(int transactionId)
        {
            var transaction = await _transactionRepo.FindAsync(transactionId);
            transaction.ReportNotTransfer();
            await _unitOfWork.SaveChangesAsync();
        }

        #endregion
    }
}