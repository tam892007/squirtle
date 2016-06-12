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
using BSE365.Mappings;
using BSE365.Model.Entities;
using BSE365.Model.Enum;
using BSE365.Repository.DataContext;
using BSE365.Repository.Helper;
using BSE365.Repository.Repositories;
using BSE365.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BSE365.Api
{
    //[Authorize]
    [RoutePrefix("api/trade")]
    public class TradeController : ApiController
    {
        private readonly IRepositoryAsync<Account> _accountRepo;
        private readonly IRepositoryAsync<MoneyTransaction> _transactionRepo;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IRepositoryAsync<WaitingGiver> _waitingGiverRepo;
        private readonly IRepositoryAsync<WaitingReceiver> _waitingReceiverRepo;

        private UserProfileRepository _userRepo;

        public TradeController(
            IUnitOfWorkAsync unitOfWork,
            IRepositoryAsync<Account> accountRepo,
            IRepositoryAsync<MoneyTransaction> transactionRepo,
            IRepositoryAsync<WaitingGiver> waitingGiverRepo,
            IRepositoryAsync<WaitingReceiver> waitingReceiverRepo)
        {
            _unitOfWork = unitOfWork;
            _accountRepo = accountRepo;
            _transactionRepo = transactionRepo;
            _waitingGiverRepo = waitingGiverRepo;
            _waitingReceiverRepo = waitingReceiverRepo;


            _userRepo = new UserProfileRepository();
        }

        [HttpGet]
        [Route("InitDatabase")]
        public async Task<IHttpActionResult> InitDatabase(int key = 0)
        {
            switch (key)
            {
                case 10:
                    Repository.BSE365AuthContextMigration.Configuration.InitData(
                        new BSE365AuthContext());
                    Repository.BSE365ContextMigration.Configuration.CreateAccount(
                        new BSE365Context());
                    Repository.BSE365ContextMigration.Configuration.QueueWaitingList(
                        new BSE365Context());
                    break;
                case 1:
                    Repository.BSE365AuthContextMigration.Configuration.InitData(
                        new BSE365AuthContext());
                    break;
                case 2:
                    Repository.BSE365ContextMigration.Configuration.CreateAccount(
                        new BSE365Context());
                    break;
                case 0:
                    Repository.BSE365ContextMigration.Configuration.ClearWaitingTransactionData(
                        new BSE365Context());
                    break;
            }
            return Ok();
        }


        [HttpGet]
        [Route("MapGiversAndReceivers")]
        public async Task<IHttpActionResult> MapGiversAndReceivers()
        {
            StoreHelper.MapWaitingGiversAndWaitingReceivers();
            return Ok();
        }

        [HttpGet]
        [Route("UpdateTransactions")]
        public async Task<IHttpActionResult> UpdateTransactions()
        {
            StoreHelper.UpdateNotTransferedTransactions();
            StoreHelper.UpdateNotConfirmedTransactions();
            return Ok();
        }


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

        [HttpPost]
        [Route("SetAccountPriority")]
        public async Task<IHttpActionResult> SetAccountPriority(TradeAccountVM.SetPriorityVM priority)
        {
            await SetAccountPriorityAsync(priority);
            return Ok();
        }

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
        [Route("QueryWaitingGivers")]
        public async Task<IHttpActionResult> QueryWaitingGivers(FilterVM filter)
        {
            var result = await QueryWaitingGiversAsync(filter);
            return Ok(result);
        }

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

        [HttpPost]
        [Route("QueryPunishment")]
        public async Task<IHttpActionResult> QueryPunishment(FilterVM filter)
        {
            var result = await QueryPunishmentAsync(filter);
            return Ok(result);
        }

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
            IQueryFluent<Account> query = null;
            if (filter.Search.PredicateObject == null)
            {
                query = _accountRepo.Query();
            }
            else
            {
                var userName = filter.Search.PredicateObject.Value<string>("userName");
                query = _accountRepo.Query(x => x.UserName.Contains(userName));
            }

            var rawData =
                await
                    query.OrderBy(x => x.OrderBy(a => a.UserName))
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
            var account = await _accountRepo.Queryable()
                .Include(x => x.UserInfo)
                .Include(x => x.WaitingGivers)
                .Include(x => x.WaitingReceivers)
                .Where(x => x.UserName == username).FirstAsync();
            account.ChangeState(state.State);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task QueueGiveAsync(string key)
        {
            var username = string.IsNullOrEmpty(key) ? User.Identity.GetUserName() : key;
            var account = await _accountRepo.Queryable()
                .Include(x => x.UserInfo)
                .Where(x => x.UserName == username).FirstAsync();

            var _ctx = new BSE365AuthContext();
            var _userManager = new UserManager<User>(new UserStore<User>(_ctx));

            var userId = User.Identity.GetUserId();
            var user = await _userManager.FindByIdAsync(userId);


            _unitOfWork.BeginTransaction(IsolationLevel.RepeatableRead);
            using (var authTransaction = _ctx.Database.BeginTransaction(IsolationLevel.RepeatableRead))
            {
                try
                {
                    account.QueueGive();

                    user.GiveQueued();

                    await _userManager.UpdateAsync(user);

                    await _unitOfWork.SaveChangesAsync();
                    await _ctx.SaveChangesAsync();

                    authTransaction.Commit();
                    _unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    authTransaction.Rollback();
                    _unitOfWork.Rollback();
                }
            }
        }

        private async Task QueueReceiveAsync(string key)
        {
            var username = string.IsNullOrEmpty(key) ? User.Identity.GetUserName() : key;
            var account = await _accountRepo.Queryable()
                .Include(x => x.UserInfo)
                .Where(x => x.UserName == username).FirstAsync();
            account.QueueReceive();
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<List<WaitingAccountVM>> QueryWaitingGiversAsync(FilterVM filter)
        {
            var expression = WaitingAccountVMMapping.GetExpToGiverVM();
            var data = await _waitingGiverRepo.Queryable()
                .OrderByDescending(x => x.Priority)
                .ThenBy(x => x.Created)
                .Select(expression)
                .ToListAsync();
            return data;
        }

        private async Task<List<WaitingAccountVM>> QueryWaitingReceiversAsync(FilterVM filter)
        {
            var expression = WaitingAccountVMMapping.GetExpToReceiverVM();
            var data = await _waitingReceiverRepo.Queryable()
                .OrderByDescending(x => x.Priority)
                .ThenBy(x => x.Created)
                .Select(expression)
                .ToListAsync();
            return data;
        }


        private async Task<List<TransactionHistoryVM>> QueryHistoryAsync(FilterVM filter)
        {
            var username = User.Identity.GetUserName();
            var expression = TransactionHistoryVMMapping.GetExpToVM(username);
            var giveTrans = _transactionRepo.Queryable()
                .Where(x => x.GiverId == username && x.Type != TransactionType.Replacement)
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

        private async Task<int> MapForReceiverAsync(WaitingAccountVM instance)
        {
            var result = StoreHelper.MapWaitingReceiver(instance.Id);
            return result;
        }

        #endregion
    }
}