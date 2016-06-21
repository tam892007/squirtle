using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
using Hangfire;
using LinqKit;

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
        [Route("AbandonTransaction")]
        public async Task<IHttpActionResult> AbandonTransaction(MoneyTransactionVM.Receiver transaction)
        {
            var result = await AbandonTransactionAsync(transaction);
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

        [HttpPost]
        [Route("QueryUserHistory")]
        public async Task<IHttpActionResult> QueryUserHistory(FilterVM filter)
        {
            var result = await QueryUserHistoryAsync(filter);
            return Ok(result);
        }

        [HttpPost]
        [Route("QueryUserPunishment")]
        public async Task<IHttpActionResult> QueryUserPunishment(FilterVM filter)
        {
            var result = await QueryUserPunishmentAsync(filter);
            return Ok(result);
        }

        [HttpPost]
        [Route("QueryUserBonus")]
        public async Task<IHttpActionResult> QueryUserBonus(FilterVM filter)
        {
            var result = await QueryUserBonusAsync(filter);
            return Ok(result);
        }

        [HttpPost]
        [Route("TransactionDetails")]
        public async Task<IHttpActionResult> TransactionDetails(int key)
        {
            var result = await TransactionDetailsAsync(key);
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
                .Include(x => x.Giver.UserInfo)
                .Include(x => x.Receiver.UserInfo)
                .FirstAsync(x => x.Id == tran.Id);
            _unitOfWork.BeginTransaction(IsolationLevel.RepeatableRead);
            try
            {
                transaction.MoneyTransfered(tran.AttachmentUrl);
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                throw;
            }
            ////Send email
            var request = System.Web.HttpContext.Current.Request;
            var baseUri = request.Url.AbsoluteUri.Replace(request.Url.PathAndQuery, string.Empty);
            BackgroundJob.Enqueue(() => NotificationHelper.NotifyTransactionGived(transaction.Id,
                transaction.GiverId, transaction.ReceiverId));
            BackgroundJob.Enqueue(() => EmailHelper.NotifyTransactionGived(transaction.Id,
                transaction.GiverId, transaction.Giver.UserInfo.Email,
                transaction.ReceiverId, transaction.Receiver.UserInfo.Email,
                baseUri));
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
            _unitOfWork.BeginTransaction(IsolationLevel.RepeatableRead);
            try
            {
                transaction.MoneyReceived(otherGivingTransactionsInCurrentTransaction,
                    otherReceivingTransactionsInCurrentTransaction,
                    giverParentInfos);
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                throw;
            }
            ////Send email
            var request = System.Web.HttpContext.Current.Request;
            var baseUri = request.Url.AbsoluteUri.Replace(request.Url.PathAndQuery, string.Empty);
            BackgroundJob.Enqueue(() => NotificationHelper.NotifyTransactionReceived(transaction.Id,
                transaction.GiverId, transaction.ReceiverId));
            BackgroundJob.Enqueue(() => EmailHelper.NotifyTransactionReceived(transaction.Id,
                transaction.GiverId, transaction.Giver.UserInfo.Email,
                transaction.ReceiverId, transaction.Receiver.UserInfo.Email,
                baseUri));
            return transaction.UpdateVm(tran);
        }

        private async Task<MoneyTransactionVM.Giver> ReportNotTransferAsync(MoneyTransactionVM.Giver tran)
        {
            var transaction = await _transactionRepo.Queryable()
                .Include(x => x.Receiver)
                .Include(x => x.Giver)
                .FirstAsync(x => x.Id == tran.Id);
            _unitOfWork.BeginTransaction(IsolationLevel.RepeatableRead);
            try
            {
                transaction.ReportNotTransfer();
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                throw;
            }
            return transaction.UpdateVm(tran);
        }

        private async Task<MoneyTransactionVM.Receiver> AbandonTransactionAsync(MoneyTransactionVM.Receiver tran)
        {
            var transaction = await _transactionRepo.Queryable()
                .Include(x => x.Giver.UserInfo)
                .Include(x => x.Giver.WaitingGivers)
                .Include(x => x.Receiver.UserInfo)
                .Include(x => x.Receiver.WaitingReceivers)
                .FirstAsync(x => x.Id == tran.Id);
            _unitOfWork.BeginTransaction(IsolationLevel.RepeatableRead);
            try
            {
                transaction.Abandon();
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                throw;
            }
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
            IQueryFluent<MoneyTransaction> query = _transactionRepo.Query();
            if (filter.Search.PredicateObject != null)
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
            IQueryFluent<MoneyTransaction> query =
                _transactionRepo.Query(x => x.State == TransactionState.ReportedNotTransfer);
            if (filter.Search.PredicateObject != null)
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
            _unitOfWork.BeginTransaction(IsolationLevel.RepeatableRead);
            try
            {
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
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                throw;
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

        private async Task<PageViewModel<MoneyTransactionVM.Base>> QueryUserHistoryAsync(FilterVM filter)
        {
            return await QueryUserTransactionAsync(filter);

            int totalPageCount;
            var username = User.Identity.GetUserName();
            IQueryFluent<MoneyTransaction> query =
                _transactionRepo.Query(x => x.GiverId == username || x.ReceiverId == username);
            if (filter.Search.PredicateObject != null)
            {
                var summary = filter.Search.PredicateObject.Value<string>("$");
                if (!string.IsNullOrWhiteSpace(summary))
                {
                    int id;
                    if (int.TryParse(summary, out id))
                    {
                        query = _transactionRepo.Query(x =>
                            (x.GiverId == username && (x.Id == id || x.ReceiverId.Contains(summary))) ||
                            (x.ReceiverId == username && (x.Id == id || x.GiverId.Contains(summary))));
                    }
                    else
                    {
                        query = _transactionRepo.Query(x =>
                            (x.GiverId == username && x.ReceiverId.Contains(summary)) ||
                            (x.ReceiverId == username && x.GiverId.Contains(summary)));
                    }
                }
            }

            var selectExpression = MoneyTransactionVMMapping.GetExpToVM(username);
            var data = await query.OrderBy(x => x.OrderByDescending(a => a.Id))
                .SelectQueryable(selectExpression, filter.Pagination.Start/filter.Pagination.Number + 1,
                    filter.Pagination.Number, out totalPageCount)
                .ToListAsync();

            var page = new PageViewModel<MoneyTransactionVM.Base>
            {
                Data = data,
                TotalItems = totalPageCount
            };

            return page;
        }

        private async Task<PageViewModel<MoneyTransactionVM.Base>> QueryUserPunishmentAsync(FilterVM filter)
        {
            int totalPageCount;
            var username = User.Identity.GetUserName();

            Expression<Func<MoneyTransaction, bool>> byGiver = x => x.GiverId == username;
            Expression<Func<MoneyTransaction, bool>> byType = x => x.Type == TransactionType.Replacement;

            var predicate = byGiver.And(byType);

            if (filter.Search.PredicateObject != null)
            {
                var summary = filter.Search.PredicateObject.Value<string>("$");
                if (!string.IsNullOrWhiteSpace(summary))
                {
                    int id;
                    Expression<Func<MoneyTransaction, bool>> byReceiverContains = x => x.ReceiverId.Contains(summary);
                    if (int.TryParse(summary, out id))
                    {
                        Expression<Func<MoneyTransaction, bool>> byId = x => x.Id == id;
                        predicate = predicate.And(byId.Or(byReceiverContains));
                    }
                    else
                    {
                        predicate = predicate.And(byReceiverContains);
                    }
                }
            }
            var selectExpression = MoneyTransactionVMMapping.GetExpToVM(username);
            var data = await _transactionRepo.Query(predicate)
                .OrderBy(x => x.OrderByDescending(a => a.Id))
                .SelectQueryable(selectExpression,
                    filter.Pagination.Start/filter.Pagination.Number + 1, filter.Pagination.Number, out totalPageCount)
                .ToListAsync();

            var page = new PageViewModel<MoneyTransactionVM.Base>
            {
                Data = data,
                TotalItems = totalPageCount
            };

            return page;
        }

        private async Task<PageViewModel<MoneyTransactionVM.Base>> QueryUserBonusAsync(FilterVM filter)
        {
            int totalPageCount;
            var username = User.Identity.GetUserName();

            Expression<Func<MoneyTransaction, bool>> byReceiver = x => x.ReceiverId == username;
            Expression<Func<MoneyTransaction, bool>> byType = x => x.Type == TransactionType.Bonus;

            var predicate = byReceiver.And(byType);

            if (filter.Search.PredicateObject != null)
            {
                var summary = filter.Search.PredicateObject.Value<string>("$");
                if (!string.IsNullOrWhiteSpace(summary))
                {
                    int id;
                    Expression<Func<MoneyTransaction, bool>> byGiverContains = x => x.GiverId.Contains(summary);
                    if (int.TryParse(summary, out id))
                    {
                        Expression<Func<MoneyTransaction, bool>> byId = x => x.Id == id;
                        predicate = predicate.And(byId.Or(byGiverContains));
                    }
                    else
                    {
                        predicate = predicate.And(byGiverContains);
                    }
                }
            }

            var selectExpression = MoneyTransactionVMMapping.GetExpToVM(username);
            var data = await _transactionRepo.Query(predicate)
                .OrderBy(x => x.OrderByDescending(a => a.Id))
                .SelectQueryable(selectExpression,
                    filter.Pagination.Start/filter.Pagination.Number + 1, filter.Pagination.Number, out totalPageCount)
                .ToListAsync();

            var page = new PageViewModel<MoneyTransactionVM.Base>
            {
                Data = data,
                TotalItems = totalPageCount
            };

            return page;
        }

        private async Task<PageViewModel<MoneyTransactionVM.Base>> QueryUserTransactionAsync(
            FilterVM filter, TransactionType? type = null)
        {
            int totalPageCount;
            var username = User.Identity.GetUserName();

            var predicate = PredicateBuilder.True<MoneyTransaction>();
            Expression<Func<MoneyTransaction, bool>> byGiver = x => x.GiverId == username;
            Expression<Func<MoneyTransaction, bool>> byReceiver = x => x.ReceiverId == username;

            predicate = predicate.And(byGiver.Or(byReceiver));

            if (filter.Search.PredicateObject != null)
            {
                var summary = filter.Search.PredicateObject.Value<string>("$");
                if (!string.IsNullOrWhiteSpace(summary))
                {
                    int id;
                    Expression<Func<MoneyTransaction, bool>> byGiverContains = x => x.GiverId.Contains(summary);
                    Expression<Func<MoneyTransaction, bool>> byReceiverContains = x => x.ReceiverId.Contains(summary);
                    if (int.TryParse(summary, out id))
                    {
                        Expression<Func<MoneyTransaction, bool>> byId = x => x.Id == id;
                        predicate = PredicateBuilder.False<MoneyTransaction>()
                            .Or(byGiver.And(byId.Or(byReceiverContains)))
                            .Or(byReceiver.And(byId.Or(byGiverContains)));
                    }
                    else
                    {
                        predicate = PredicateBuilder.False<MoneyTransaction>()
                            .Or(byGiver.And(byReceiverContains))
                            .Or(byReceiver.And(byGiverContains));
                    }
                }
            }
            if (type != null)
            {
                Expression<Func<MoneyTransaction, bool>> byType = x => x.Type == type.Value;
                predicate = predicate.And(byType);
            }

            var selectExpression = MoneyTransactionVMMapping.GetExpToVM(username);
            var data = await _transactionRepo.Query(predicate)
                .OrderBy(x => x.OrderByDescending(a => a.Id))
                .SelectQueryable(selectExpression,
                    filter.Pagination.Start/filter.Pagination.Number + 1, filter.Pagination.Number, out totalPageCount)
                .ToListAsync();

            var page = new PageViewModel<MoneyTransactionVM.Base>
            {
                Data = data,
                TotalItems = totalPageCount
            };

            return page;
        }

        private async Task<MoneyTransactionVM.Base> TransactionDetailsAsync(int key)
        {
            var username = User.Identity.GetUserName();
            var expression = MoneyTransactionVMMapping.GetExpToVM(username);
            var result = await _transactionRepo.Queryable()
                .Where(x => x.Id == key)
                .Select(expression)
                .FirstAsync();
            return result;
        }

        #endregion
    }
}