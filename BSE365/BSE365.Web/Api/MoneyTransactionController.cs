using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using BSE365.Base.Infrastructures;
using BSE365.Base.Repositories.Contracts;
using BSE365.Base.UnitOfWork;
using BSE365.Base.UnitOfWork.Contracts;
using BSE365.Common.Helper;
using BSE365.Helper;
using BSE365.Mappings;
using BSE365.Model.Entities;
using BSE365.Model.Enum;
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
        private IRepositoryAsync<Account> _accountRepo;
        private IRepositoryAsync<UserInfo> _userInfoRepo;
        private UserProfileRepository _userRepo;

        #region

        #endregion

        public MoneyTransactionController(
            IUnitOfWorkAsync unitOfWork,
            IRepositoryAsync<MoneyTransaction> transactionRepo,
            IRepositoryAsync<Account> accountRepo,
            IRepositoryAsync<UserInfo> userInfoRepo)
        {
            _unitOfWork = unitOfWork;
            _transactionRepo = transactionRepo;
            _accountRepo = accountRepo;
            _userInfoRepo = userInfoRepo;
            _userRepo = new UserProfileRepository();
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

        [HttpPost]
        [Route("AbadonTransaction")]
        public async Task<IHttpActionResult> AbadonTransaction(MoneyTransactionVM.Receiver transaction)
        {
            var result = await AbadonTransactionAsync(transaction);
            return Ok(result);
        }

        [HttpPost]
        [Route("UpdateImg")]
        public async Task<IHttpActionResult> UpdateImg(MoneyTransactionVM.Receiver transaction)
        {
            var result = await UpdateImgAsync(transaction);
            return Ok(result);
        }

        [HttpPost]
        [Route("Upload")]
        public async Task<IHttpActionResult> Upload()
        {
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                throw new HttpResponseException
                    (Request.CreateResponse(HttpStatusCode.UnsupportedMediaType));
            }
            var username = User == null || User.Identity != null ? "Transactions" : User.Identity.GetUserName();
            var url = await FileHelper.Upload(Request, username);
            var result = new {Url = url};
            return Ok(result);
        }

        [HttpPost]
        [Route("History")]
        public async Task<IHttpActionResult> History(TransactionHistoryVM instance)
        {
            var result = await HistoryAsync(instance);
            return Ok(result);
        }

        [HttpPost]
        [Route("QueryTransaction")]
        public async Task<IHttpActionResult> QueryTransaction(FilterVM filter)
        {
            var result = await QueryTransactionAsync(filter);
            return Ok(result);
        }

        [HttpPost]
        [Route("ReportedTransactions")]
        public async Task<IHttpActionResult> ReportedTransactions(FilterVM filter)
        {
            var result = await ReportedTransactionsAsync(filter);
            return Ok(result);
        }

        [HttpPost]
        [Route("ApplyReport")]
        public async Task<IHttpActionResult> ApplyReport(MoneyTransactionVM.Reported instance)
        {
            await ApplyReportAsync(instance);
            return Ok();
        }

        [HttpPost]
        [Route("QueryPunishment")]
        public async Task<IHttpActionResult> QueryPunishment(FilterVM filter)
        {
            var result = await QueryPunishmentAsync(filter);
            return Ok(result);
        }

        [HttpPost]
        [Route("QueryBonus")]
        public async Task<IHttpActionResult> QueryBonus(FilterVM filter)
        {
            var result = await QueryBonusAsync(filter);
            return Ok(result);
        }

        #region internal method

        private async Task<List<MoneyTransactionVM.Receiver>> GiveRequestedAsync()
        {
            var username = User.Identity.GetUserName();
            var expression = MoneyTransactionVMMapping.GetExpToReceiverVM();
            var data = await _transactionRepo.Queryable()
                .Include(x => x.Receiver.UserInfo)
                .Where(x => x.GiverId == username && x.WaitingGiverId == x.Giver.CurrentTransactionGroupId)
                .Select(expression)
                .ToListAsync();
            return data;
        }

        private async Task<List<MoneyTransactionVM.Giver>> ReceiveRequestedAsync()
        {
            var username = User.Identity.GetUserName();
            var expression = MoneyTransactionVMMapping.GetExpToGiverVM();
            var data = await _transactionRepo.Queryable()
                .Include(x => x.Giver.UserInfo)
                .Where(x => x.ReceiverId == username && x.WaitingReceiverId == x.Receiver.CurrentTransactionGroupId)
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
                .Include(x => x.Giver.UserInfo)
                .Include(x => x.Receiver.UserInfo)
                .Include(x => x.Giver.WaitingGivers)
                .Include(x => x.Receiver.WaitingReceivers)
                .FirstAsync(x => x.Id == tran.Id);
            var otherGivingTransactionsInCurrentTransaction = await _transactionRepo.Queryable()
                .Where(x => x.WaitingGiverId == transaction.WaitingGiverId && x.Id != transaction.Id)
                .ToListAsync();
            var otherReceivingTransactionsInCurrentTransaction = await _transactionRepo.Queryable()
                .Where(x => x.WaitingReceiverId == transaction.WaitingReceiverId && x.Id != transaction.Id)
                .ToListAsync();
            var giverParentInfoIds = await _userRepo.GetParentInfoIdsFromTreePath(transaction.Giver.UserInfo.TreePath);
            var giverParentInfos = await _userInfoRepo.Queryable()
                .Where(x => giverParentInfoIds.Contains(x.Id)).ToListAsync();
            transaction.MoneyReceived(otherGivingTransactionsInCurrentTransaction,
                otherReceivingTransactionsInCurrentTransaction,
                giverParentInfos);
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

        private async Task<MoneyTransactionVM.Receiver> AbadonTransactionAsync(MoneyTransactionVM.Receiver tran)
        {
            var transaction = await _transactionRepo.Queryable()
                .Include(x => x.Giver.UserInfo)
                .Include(x => x.Giver.WaitingGivers)
                .Include(x => x.Receiver.UserInfo)
                .Include(x => x.Receiver.WaitingReceivers)
                .FirstAsync(x => x.Id == tran.Id);
            transaction.Abadon();
            await _unitOfWork.SaveChangesAsync();
            return transaction.UpdateVm(tran);
        }

        private async Task<MoneyTransactionVM.Receiver> UpdateImgAsync(MoneyTransactionVM.Receiver tran)
        {
            var transaction = await _transactionRepo.Queryable()
                .FirstAsync(x => x.Id == tran.Id);
            transaction.AttachmentUrl = tran.AttachmentUrl;
            transaction.LastModified = DateTime.Now;
            transaction.ObjectState = ObjectState.Modified;
            await _unitOfWork.SaveChangesAsync();
            return tran;
        }

        private async Task<List<MoneyTransactionVM.Simple>> HistoryAsync(TransactionHistoryVM instance)
        {
            var username = User.Identity.GetUserName();
            if (instance.Type == AccountState.InGiveTransaction)
            {
                var expression = MoneyTransactionVMMapping.GetExpToReceiverVM();
                var data = await _transactionRepo.Queryable().Where(x =>
                    x.WaitingGiverId == instance.Id && x.GiverId == username)
                    .Include(x => x.Receiver.UserInfo)
                    .Select(expression)
                    .ToListAsync();
                return data.Select(x => x as MoneyTransactionVM.Simple).ToList();
            }
            else
            {
                var expression = MoneyTransactionVMMapping.GetExpToGiverVM();
                var data = await _transactionRepo.Queryable().Where(x =>
                    x.WaitingReceiverId == instance.Id && x.ReceiverId == username)
                    .Include(x => x.Giver.UserInfo)
                    .Select(expression)
                    .ToListAsync();
                return data.Select(x => x as MoneyTransactionVM.Simple).ToList();
            }
        }

        private async Task<PageViewModel<MoneyTransactionVM.Base>> QueryTransactionAsync(FilterVM filter)
        {
            int totalPageCount;
            IQueryFluent<MoneyTransaction> query = null;
            if (filter.Search.PredicateObject == null)
            {
                query = _transactionRepo.Query();
            }
            else
            {
                var summary = filter.Search.PredicateObject.Value<string>("$");
                if (!string.IsNullOrWhiteSpace(summary))
                {
                    int id;
                    if (int.TryParse(summary, out id))
                    {
                        query = _transactionRepo.Query(
                            x => x.Id == id || x.GiverId.Contains(summary) || x.ReceiverId.Contains(summary));
                    }
                    else
                    {
                        query = _transactionRepo.Query(
                            x => x.GiverId.Contains(summary) || x.ReceiverId.Contains(summary));
                    }
                }
                else
                {
                    query = _transactionRepo.Query();
                }
            }

            var expression = MoneyTransactionVMMapping.GetExpToVM();
            var data = await query.OrderBy(x => x.OrderByDescending(a => a.Id))
                .SelectQueryable(expression, filter.Pagination.Start/filter.Pagination.Number + 1,
                    filter.Pagination.Number, out totalPageCount)
                .ToListAsync();

            var page = new PageViewModel<MoneyTransactionVM.Base>
            {
                Data = data,
                TotalItems = totalPageCount
            };

            return page;
        }

        private async Task<PageViewModel<MoneyTransactionVM.Base>> ReportedTransactionsAsync(FilterVM filter)
        {
            int totalPageCount;
            IQueryFluent<MoneyTransaction> query = null;
            if (filter.Search.PredicateObject == null)
            {
                query = _transactionRepo.Query();
            }
            else
            {
                var summary = filter.Search.PredicateObject.Value<string>("$");
                if (!string.IsNullOrWhiteSpace(summary))
                {
                    int id;
                    if (int.TryParse(summary, out id))
                    {
                        query = _transactionRepo.Query(x =>
                            x.State == TransactionState.ReportedNotTransfer &&
                            (x.Id == id || x.GiverId.Contains(summary) || x.ReceiverId.Contains(summary)));
                    }
                    else
                    {
                        query = _transactionRepo.Query(x =>
                            x.State == TransactionState.ReportedNotTransfer &&
                            (x.GiverId.Contains(summary) || x.ReceiverId.Contains(summary)));
                    }
                }
                else
                {
                    query = _transactionRepo.Query();
                }
            }

            var expression = MoneyTransactionVMMapping.GetExpToVM();
            var data = await query.OrderBy(x => x.OrderBy(a => a.Id))
                .SelectQueryable(expression, filter.Pagination.Start/filter.Pagination.Number + 1,
                    filter.Pagination.Number, out totalPageCount)
                .ToListAsync();

            var page = new PageViewModel<MoneyTransactionVM.Base>
            {
                Data = data,
                TotalItems = totalPageCount
            };

            return page;
        }

        private async Task ApplyReportAsync(MoneyTransactionVM.Reported instance)
        {
            var transaction = await _transactionRepo.Queryable()
                .Include(x => x.Giver.UserInfo)
                .Include(x => x.Receiver.UserInfo)
                .Where(x => x.Id == instance.Id)
                .FirstAsync();
            var otherGivingTransactionsInCurrentTransaction = _transactionRepo.Queryable()
                .Where(x =>
                    x.WaitingGiverId == transaction.WaitingGiverId && x.Id != transaction.Id)
                .ToList();
            Account giverParentAccount = null;
            var giverParentId = transaction.Giver.UserInfo.ParentId;
            if (!string.IsNullOrEmpty(giverParentId))
            {
                var giverParentAuthAccount = await _userRepo.FindUser(giverParentId);
                giverParentAccount = _accountRepo.Queryable()
                    .FirstOrDefault(x => x.UserName == giverParentAuthAccount.UserName);
            }
            switch (instance.Result)
            {
                case MoneyTransactionVM.ReportResult.Default:
                    break;
                case MoneyTransactionVM.ReportResult.GiverTrue:
                    var giverParentInfoIds =
                        await _userRepo.GetParentInfoIdsFromTreePath(transaction.Giver.UserInfo.TreePath);
                    var giverParentInfos = await _userInfoRepo.Queryable()
                        .Where(x => giverParentInfoIds.Contains(x.Id)).ToListAsync();
                    transaction.NotConfirm(otherGivingTransactionsInCurrentTransaction, giverParentInfos);
                    await _unitOfWork.SaveChangesAsync();
                    break;
                case MoneyTransactionVM.ReportResult.ReceiverTrue:
                    transaction.NotTransfer(giverParentAccount);
                    await _unitOfWork.SaveChangesAsync();
                    break;
                case MoneyTransactionVM.ReportResult.BothTrue:
                    await MoneyReceivedAsync(new MoneyTransactionVM.Giver {Id = transaction.Id});
                    break;
                case MoneyTransactionVM.ReportResult.BothFalse:
                    transaction.Failed();
                    await _unitOfWork.SaveChangesAsync();
                    break;
            }
        }

        private async Task<List<MoneyTransactionVM.Punishment>> QueryPunishmentAsync(FilterVM filter)
        {
            var username = User.Identity.GetUserName();
            var expression = MoneyTransactionVMMapping.GetExpToPunismentVM();
            var data = await _transactionRepo.Queryable()
                .Where(x => x.GiverId == username && x.Type == TransactionType.Replacement)
                .OrderByDescending(x => x.Created)
                .Select(expression)
                .ToListAsync();
            return data;
        }

        private async Task<List<MoneyTransactionVM.Giver>> QueryBonusAsync(FilterVM filter)
        {
            var username = User.Identity.GetUserName();
            var expression = MoneyTransactionVMMapping.GetExpToGiverVM();
            var data = await _transactionRepo.Queryable()
                .Where(x => x.ReceiverId == username && x.Type == TransactionType.Bonus)
                .OrderByDescending(x => x.Created)
                .Select(expression)
                .ToListAsync();
            return data;
        }

        #endregion
    }
}