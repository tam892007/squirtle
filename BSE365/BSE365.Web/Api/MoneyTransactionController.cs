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
        /// <param name="transaction"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("MoneyTransfered")]
        public async Task<IHttpActionResult> MoneyTransfered(MoneyTransactionVM.Receiver transaction)
        {
            var result = await MoneyTransferedAsync(transaction);
            return Ok(result);
        }

        /// <summary>
        /// xác nhận đã nhận
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("MoneyReceived")]
        public async Task<IHttpActionResult> MoneyReceived(MoneyTransactionVM.Giver transaction)
        {
            var result = await MoneyReceivedAsync(transaction);
            return Ok(result);
        }

        /// <summary>
        /// report việc không gửi nhưng lại xác nhận đã gửi trên hệ thống
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ReportNotTransfer")]
        public async Task<IHttpActionResult> ReportNotTransfer(MoneyTransactionVM.Giver transaction)
        {
            var result = await ReportNotTransferAsync(transaction);
            return Ok(result);
        }

        #region internal method

        private async Task<List<MoneyTransactionVM.Receiver>> GiveRequestedAsync()
        {
            var username = User.Identity.GetUserName();
            var expression = MoneyTransactionVMMapping.GetExpToReceiverVM();
            var data = await _transactionRepo.Queryable().Where(x =>
                !x.IsEnd
                && x.GiverId == username)
                .Include(x => x.Receiver.UserInfo)
                .Select(expression)
                .ToListAsync();
            return data;
        }

        private async Task<List<MoneyTransactionVM.Giver>> ReceiveRequestedAsync()
        {
            var username = User.Identity.GetUserName();
            var expression = MoneyTransactionVMMapping.GetExpToGiverVM();
            var data = await _transactionRepo.Queryable().Where(x =>
                !x.IsEnd
                && x.ReceiverId == username)
                .Include(x => x.Giver.UserInfo)
                .Select(expression)
                .ToListAsync();
            return data;
        }

        private async Task<MoneyTransactionVM.Receiver> MoneyTransferedAsync(MoneyTransactionVM.Receiver tran)
        {
            var transaction = await _transactionRepo.Queryable()
                .Include(x => x.Receiver)
                .FirstAsync(x => x.Id == tran.Id);
            transaction.MoneyTransfered(tran.AttachmentUrl);
            await _unitOfWork.SaveChangesAsync();
            return transaction.UpdateVm(tran);
        }

        private async Task<MoneyTransactionVM.Giver> MoneyReceivedAsync(MoneyTransactionVM.Giver tran)
        {
            var transaction = await _transactionRepo.Queryable()
                .Include(x => x.Receiver)
                .Include(x => x.Giver)
                .FirstAsync(x => x.Id == tran.Id);
            transaction.MoneyReceived();
            await _unitOfWork.SaveChangesAsync();
            return transaction.UpdateVm(tran);
        }

        private async Task<MoneyTransactionVM.Giver> ReportNotTransferAsync(MoneyTransactionVM.Giver tran)
        {
            var transaction = await _transactionRepo.Queryable()
                .Include(x => x.Receiver)
                .Include(x => x.Giver)
                .FirstAsync(x => x.Id == tran.Id);
            transaction.ReportNotTransfer();
            await _unitOfWork.SaveChangesAsync();
            return transaction.UpdateVm(tran);
        }

        #endregion
    }
}