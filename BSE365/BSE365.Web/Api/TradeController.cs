using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BSE365.Base.DataContext.Contracts;
using BSE365.Base.Infrastructures;
using BSE365.Base.Repositories.Contracts;
using BSE365.Base.UnitOfWork.Contracts;
using BSE365.Mappings;
using BSE365.Model.Entities;
using BSE365.Model.Enum;
using BSE365.Repository.Helper;
using BSE365.Repository.Repositories;
using BSE365.ViewModels;
using Microsoft.AspNet.Identity;

namespace BSE365.Api
{
    //[Authorize]
    [RoutePrefix("api/trade")]
    public class TradeController : ApiController
    {
        IUnitOfWorkAsync _unitOfWork;
        IRepositoryAsync<Account> _accountRepo;
        IRepositoryAsync<MoneyTransaction> _transactionRepo;
        IRepositoryAsync<WaitingGiver> _waitingGiverRepo;
        IRepositoryAsync<WaitingReceiver> _waitingReceiverRepo;
        IRepositoryAsync<MoneyTransferGroup> _groupRepo;

        public TradeController(
            IUnitOfWorkAsync unitOfWork,
            IRepositoryAsync<Account> accountRepo,
            IRepositoryAsync<MoneyTransaction> transactionRepo,
            IRepositoryAsync<WaitingGiver> waitingGiverRepo,
            IRepositoryAsync<WaitingReceiver> waitingReceiverRepo,
            IRepositoryAsync<MoneyTransferGroup> groupRepo)
        {
            _unitOfWork = unitOfWork;
            _accountRepo = accountRepo;
            _transactionRepo = transactionRepo;
            _waitingGiverRepo = waitingGiverRepo;
            _waitingReceiverRepo = waitingReceiverRepo;
            _groupRepo = groupRepo;
        }

        [HttpGet]
        [Route("InitDatabase")]
        public async Task<IHttpActionResult> InitDatabase(int key = 0)
        {
            switch (key)
            {
                case 10:
                    BSE365.Repository.BSE365AuthContextMigration.Configuration.InitData(
                        new BSE365.Repository.DataContext.BSE365AuthContext());
                    BSE365.Repository.BSE365ContextMigration.Configuration.CreateAccount(
                        new BSE365.Repository.DataContext.BSE365Context());
                    BSE365.Repository.BSE365ContextMigration.Configuration.QueueWaitingList(
                        new BSE365.Repository.DataContext.BSE365Context());
                    break;
                case 1:
                    BSE365.Repository.BSE365AuthContextMigration.Configuration.InitData(
                        new BSE365.Repository.DataContext.BSE365AuthContext());
                    break;
                case 2:
                    BSE365.Repository.BSE365ContextMigration.Configuration.CreateAccount(
                        new BSE365.Repository.DataContext.BSE365Context());
                    break;
                case 0:
                    BSE365.Repository.BSE365ContextMigration.Configuration.ClearWaitingTransactionData(
                        new BSE365.Repository.DataContext.BSE365Context());
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
        /// kiểm tra status của account
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
        /// đăng ký cho
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
        /// đăng ký nhận
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

        #region internal method

        private async Task<List<TradeAccountVM>> QueryAccountAsync(FilterVM filter)
        {
            var rawData = await _accountRepo.Queryable()
                .Include(x => x.UserInfo)
                .ToListAsync();
            var data = rawData.Select(x => x.ToVM()).ToList();
            return data;
        }

        private async Task<TradeAccountVM> AccountStatusAsync(string key)
        {
            var username = string.IsNullOrEmpty(key) ? User.Identity.GetUserName() : key;
            var account = await _accountRepo.Queryable()
                .Include(x => x.UserInfo)
                .Where(x => x.UserName == username).FirstAsync();
            var result = account.ToVM();
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
            account.QueueGive();
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<List<WaitingAccountVM>> QueryWaitingGiversAsync(FilterVM filter)
        {
            var expression = WaitingAccountVMMapping.GetExpToGiverVM();
            var data = await _waitingGiverRepo.Queryable()
                .Select(expression)
                .ToListAsync();
            return data;
        }

        private async Task<List<WaitingAccountVM>> QueryWaitingReceiversAsync(FilterVM filter)
        {
            var expression = WaitingAccountVMMapping.GetExpToReceiverVM();
            var data = await _waitingReceiverRepo.Queryable()
                .Select(expression)
                .ToListAsync();
            return data;
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


        private async Task<List<TransactionHistoryVM>> QueryHistoryAsync(FilterVM filter)
        {
            var username = User.Identity.GetUserName();
            var expression = TransactionHistoryVMMapping.GetExpToVM(username);
            var giveTrans = _transactionRepo.Queryable()
                .Where(x => x.GiverId == username)
                .GroupBy(x => x.WaitingReceiverId)
                .Select(expression);

            var receiveTrans = _transactionRepo.Queryable()
                .Where(x => x.ReceiverId == username)
                .GroupBy(x => x.WaitingReceiverId)
                .Select(expression);

            var data = await giveTrans.Union(receiveTrans)
                .ToListAsync();
            data.ForEach(x => x.AccountId = username);
            return data;
        }

        #endregion
    }
}