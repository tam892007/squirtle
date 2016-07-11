using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using BSE365.Base.Infrastructures;
using BSE365.Base.Repositories.Contracts;
using BSE365.Base.UnitOfWork.Contracts;
using BSE365.Common.Constants;
using BSE365.Mappings;
using BSE365.Model.Entities;
using BSE365.Model.Enum;
using BSE365.Repository.DataContext;
using BSE365.Repository.Repositories;
using BSE365.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Hangfire;
using BSE365.Helper;

namespace BSE365.Api
{
    [Authorize]
    [RoutePrefix("api/trade")]
    public class TradeController : ApiController
    {
        private readonly IRepositoryAsync<Account> _accountRepo;
        private readonly IRepositoryAsync<MoneyTransaction> _transactionRepo;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IRepositoryAsync<WaitingGiver> _waitingGiverRepo;
        private readonly IRepositoryAsync<WaitingReceiver> _waitingReceiverRepo;
        private readonly IRepositoryAsync<Message> _messageRepo;

        private UserProfileRepository _userRepo;

        public TradeController(
            IUnitOfWorkAsync unitOfWork,
            IRepositoryAsync<Account> accountRepo,
            IRepositoryAsync<MoneyTransaction> transactionRepo,
            IRepositoryAsync<WaitingGiver> waitingGiverRepo,
            IRepositoryAsync<WaitingReceiver> waitingReceiverRepo,
            IRepositoryAsync<Message> messageRepo)
        {
            _unitOfWork = unitOfWork;
            _accountRepo = accountRepo;
            _transactionRepo = transactionRepo;
            _waitingGiverRepo = waitingGiverRepo;
            _waitingReceiverRepo = waitingReceiverRepo;

            _messageRepo = messageRepo;

            _userRepo = new UserProfileRepository();
        }

        [Authorize(Roles = UserRolesText.SuperAdmin)]
        [HttpPost]
        [Route("QueryAccount")]
        public async Task<IHttpActionResult> QueryAccount(FilterVM filter)
        {
            var result = await QueryAccountAsync(filter);
            return Ok(result);
        }

        /// <summary>
        ///     kiểm tra status của account
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("AccountStatus")]
        public async Task<IHttpActionResult> AccountStatus(string key = null)
        {
            var result = await AccountStatusAsync(key);
            return Ok(result);
        }

        [Authorize(Roles = UserRolesText.SuperAdmin)]
        [HttpPost]
        [Route("SetAccountPriority")]
        public async Task<IHttpActionResult> SetAccountPriority(TradeAccountVM.SetPriorityVM priority)
        {
            await SetAccountPriorityAsync(priority);
            return Ok();
        }

        [Authorize(Roles = UserRolesText.SuperAdmin)]
        [HttpPost]
        [Route("SetAccountState")]
        public async Task<IHttpActionResult> SetAccountState(TradeAccountVM.SetStateVM state)
        {
            await SetAccountStateAsync(state);
            return Ok();
        }

        /// <summary>
        ///     đăng ký cho
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("QueueGive")]
        public async Task<IHttpActionResult> QueueGive(string key = null)
        {
            await QueueGiveAsync(key);
            return Ok();
        }

        /// <summary>
        ///     đăng ký nhận
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("QueueReceive")]
        public async Task<IHttpActionResult> QueueReceive(string key = null)
        {
            await QueueReceiveAsync(key);
            return Ok();
        }

        [HttpPost]
        [Route("ClaimBonus")]
        public async Task<IHttpActionResult> ClaimBonus(string key = null)
        {
            await ClaimBonusAsync(key);
            return Ok();
        }

        [Authorize(Roles = UserRolesText.SuperAdmin)]
        [HttpPost]
        [Route("QueryWaitingGivers")]
        public async Task<IHttpActionResult> QueryWaitingGivers(FilterVM filter)
        {
            var result = await QueryWaitingGiversAsync(filter);
            return Ok(result);
        }

        [Authorize(Roles = UserRolesText.SuperAdmin)]
        [HttpPost]
        [Route("QueryWaitingReceivers")]
        public async Task<IHttpActionResult> QueryWaitingReceivers(FilterVM filter)
        {
            var result = await QueryWaitingReceiversAsync(filter);
            return Ok(result);
        }

        [HttpPost]
        [Route("QueryHistory")]
        public async Task<IHttpActionResult> QueryHistory(FilterVM filter)
        {
            var result = await QueryHistoryAsync(filter);
            return Ok(result);
        }

        [Authorize(Roles = UserRolesText.SuperAdmin)]
        [HttpPost]
        [Route("MapForReceiver")]
        public async Task<IHttpActionResult> MapForReceiver(WaitingAccountVM instance)
        {
            var result = await MapForReceiverAsync(instance);
            return Ok(new {AmountLeft = result});
        }

        #region internal method

        private async Task<PageViewModel<TradeAccountVM>> QueryAccountAsync(FilterVM filter)
        {
            int totalPageCount;
            IQueryFluent<Account> query = _accountRepo.Query();
            if (filter.Search.PredicateObject != null)
            {
                var userName = filter.Search.PredicateObject.Value<string>("userName");
                query = _accountRepo.Query(x => x.UserName.Contains(userName));
            }

            var rawData = await query.OrderBy(x => x.OrderBy(a => a.UserName))
                .SelectPage(filter.Pagination.Start/filter.Pagination.Number + 1, filter.Pagination.Number,
                    out totalPageCount)
                .Include(x => x.UserInfo.Accounts)
                .ToListAsync();
            var data = rawData.Select(x => x.ToVM());

            var page = new PageViewModel<TradeAccountVM>
            {
                Data = data,
                TotalItems = totalPageCount
            };

            return page;
        }

        private async Task<TradeAccountVM> AccountStatusAsync(string key)
        {
            var username = string.IsNullOrEmpty(key) ? User.Identity.GetUserName() : key;
            var account = await _accountRepo.Queryable()
                .Include(x => x.UserInfo.Accounts)
                .Where(x => x.UserName == username).FirstAsync();
            var result = account.ToVM();
            var user = await _userRepo.FindUserByName(username);
            result.PinBalance = user.PinBalance;
            if (result.PinBalance < 1)
            {
                result.IsAllowGive = false;
                result.NotAllowGiveReason.Add("Not enough Pin.");
            }
            return result;
        }

        private async Task SetAccountPriorityAsync(TradeAccountVM.SetPriorityVM priority)
        {
            var username = priority.UserName;
            var account = await _accountRepo.Queryable()
                .Where(x => x.UserName == username).FirstAsync();
            account.Priority = priority.Priority;
            account.ObjectState = ObjectState.Modified;
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task SetAccountStateAsync(TradeAccountVM.SetStateVM state)
        {
            var username = state.UserName;
            _unitOfWork.BeginTransaction(IsolationLevel.RepeatableRead);
            var account = await _accountRepo.Queryable()
                .Include(x => x.UserInfo)
                .Include(x => x.WaitingGivers)
                .Include(x => x.WaitingReceivers)
                .Where(x => x.UserName == username).FirstAsync();
            if (account.IsAllowChangeState())
            {
                var log = Message.NewLog("[Account]-[ChangeState]:");
                try
                {
                    log.Messsage += string.Format("",
                        account.UserName, account.State,
                        account.UserInfo.DisplayName, account.UserInfo.State, account.UserInfo.GiveOver);
                    account.ChangeState(state.State);
                    log.Messsage += string.Format("",
                        account.UserName, account.State,
                        account.UserInfo.DisplayName, account.UserInfo.State, account.UserInfo.GiveOver);
                    //_messageRepo.Insert(log);
                    await _unitOfWork.SaveChangesAsync();
                    _unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    throw;
                }
            }
            else
            {
                throw new Exception("Current Account's State not allow to change State.");
            }
        }

        private async Task QueueGiveAsync(string key)
        {
            var username = string.IsNullOrEmpty(key) ? User.Identity.GetUserName() : key;
            _unitOfWork.BeginTransaction(IsolationLevel.RepeatableRead);
            var account = await _accountRepo.Queryable()
                .Include(x => x.UserInfo)
                .Where(x => x.UserName == username).FirstAsync();

            if (account.IsAllowQueueGive())
            {
                var userId = User.Identity.GetUserId();
                var _ctx = new BSE365AuthContext();

                using (var authTransaction = _ctx.Database.BeginTransaction(IsolationLevel.RepeatableRead))
                {
                    try
                    {
                        var user = await _ctx.Users.FirstAsync(x => x.Id == userId);
                        account.QueueGive();
                        await _unitOfWork.SaveChangesAsync();

                        user.GiveQueued();
                        await _ctx.SaveChangesAsync();

                        _unitOfWork.Commit();

                        authTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        _unitOfWork.Rollback();

                        authTransaction.Rollback();
                        throw;
                    }
                }
            }
            else
            {
                var message = string.Join(",", account.NotAllowGiveReason());
                throw new Exception(message);
            }
        }

        private async Task QueueReceiveAsync(string key)
        {
            var username = string.IsNullOrEmpty(key) ? User.Identity.GetUserName() : key;
            _unitOfWork.BeginTransaction(IsolationLevel.RepeatableRead);
            var account = await _accountRepo.Queryable()
                .Include(x => x.UserInfo)
                .Where(x => x.UserName == username).FirstAsync();
            if (account.IsAllowQueueReceive())
            {
                account.QueueReceive();

                try
                {
                    await _unitOfWork.SaveChangesAsync();
                    _unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    throw;
                }
            }
            else
            {
                var message = string.Join(",", account.NotAllowReceiveReason());
                throw new Exception(message);
            }
        }

        private async Task ClaimBonusAsync(string key)
        {
            var username = string.IsNullOrEmpty(key) ? User.Identity.GetUserName() : key;
            _unitOfWork.BeginTransaction(IsolationLevel.RepeatableRead);
            var account = await _accountRepo.Queryable()
                .Include(x => x.UserInfo)
                .Where(x => x.UserName == username).FirstAsync();
            if (account.IsAllowClaimBonus())
            {
                try
                {
                    account.ClaimBonus();
                    await _unitOfWork.SaveChangesAsync();
                    _unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    throw;
                }
            }
            else
            {
                throw new Exception("Current Account' State now allow to Claim Bonus");
            }
        }

        private async Task<PageViewModel<WaitingAccountVM>> QueryWaitingGiversAsync(FilterVM filter)
        {
            int totalPageCount;
            IQueryFluent<WaitingGiver> query = _waitingGiverRepo.Query();
            if (filter.Search.PredicateObject != null)
            {
                var userName = filter.Search.PredicateObject.Value<string>("userName");
                query = _waitingGiverRepo.Query(x => x.AccountId.Contains(userName));
            }

            var expression = WaitingAccountVMMapping.GetExpToGiverVM();
            var data = await query
                .OrderBy(x => x.OrderByDescending(a => a.Priority).ThenBy(t => t.Created))
                .SelectPage(filter.Pagination.Start/filter.Pagination.Number + 1, filter.Pagination.Number,
                    out totalPageCount).Select<WaitingGiver, WaitingAccountVM>(expression)
                .ToListAsync();

            var page = new PageViewModel<WaitingAccountVM>
            {
                Data = data,
                TotalItems = totalPageCount
            };

            return page;
        }

        private async Task<PageViewModel<WaitingAccountVM>> QueryWaitingReceiversAsync(FilterVM filter)
        {
            int totalPageCount;
            IQueryFluent<WaitingReceiver> query = _waitingReceiverRepo.Query();
            if (filter.Search.PredicateObject != null)
            {
                var userName = filter.Search.PredicateObject.Value<string>("userName");
                query = _waitingReceiverRepo.Query(x => x.AccountId.Contains(userName));
            }

            var expression = WaitingAccountVMMapping.GetExpToReceiverVM();
            var data = await query
                .OrderBy(x => x.OrderByDescending(a => a.Priority).ThenBy(t => t.Created))
                .SelectPage(filter.Pagination.Start/filter.Pagination.Number + 1, filter.Pagination.Number,
                    out totalPageCount).Select<WaitingReceiver, WaitingAccountVM>(expression)
                .ToListAsync();

            var page = new PageViewModel<WaitingAccountVM>
            {
                Data = data,
                TotalItems = totalPageCount
            };

            return page;
        }

        private async Task<List<TransactionHistoryVM>> QueryHistoryAsync(FilterVM filter)
        {
            var username = User.Identity.GetUserName();
            var expression = TransactionHistoryVMMapping.GetExpToVM(username);
            var giveTrans = _transactionRepo.Queryable()
                .Where(x =>
                    x.GiverId == username &&
                    x.Type != TransactionType.Replacement &&
                    x.Type != TransactionType.Bonus)
                .OrderByDescending(x => x.Created)
                .GroupBy(x => x.WaitingGiverId)
                .Select(expression);

            var receiveTrans = _transactionRepo.Queryable()
                .Where(x => x.ReceiverId == username)
                .OrderByDescending(x => x.Created)
                .GroupBy(x => x.WaitingReceiverId)
                .Select(expression);

            var data = await giveTrans.Union(receiveTrans)
                .OrderByDescending(x => x.BeginDate)
                .ToListAsync();
            data.ForEach(x => x.AccountId = username);
            return data;
        }

        private async Task<int> MapForReceiverAsync(WaitingAccountVM instance)
        {
            var result = StoreHelper.MapWaitingReceiver(instance.Id);
            return result;
        }

        #endregion
    }
}