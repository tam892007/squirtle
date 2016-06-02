using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BSE365.Base.DataContext.Contracts;
using BSE365.Base.Repositories.Contracts;
using BSE365.Base.UnitOfWork.Contracts;
using BSE365.Mappings;
using BSE365.Model.Entities;
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
                    BSE365.Repository.BSE365ContextMigration.Configuration.QueueWaitingList(
                        new BSE365.Repository.DataContext.BSE365Context());
                    break;
                case 5:
                    BSE365.Repository.BSE365ContextMigration.Configuration.ClearWaitingTransactionData(
                        new BSE365.Repository.DataContext.BSE365Context());
                    break;
            }
            return Ok();
        }

        /// <summary>
        /// kiểm tra status của account
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("AccountStatus")]
        public async Task<IHttpActionResult> AccountStatus()
        {
            var result = await AccountStatusAsync();
            return Ok(result);
        }

        /// <summary>
        /// đăng ký cho
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("QueueGive")]
        public async Task<IHttpActionResult> QueueGive()
        {
            await QueueGiveAsync();
            return Ok();
        }

        /// <summary>
        /// đăng ký nhận
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("QueueReceive")]
        public async Task<IHttpActionResult> QueueReceive()
        {
            await QueueReceiveAsync();
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

        #region internal method

        private async Task QueueGiveAsync()
        {
            var account = await _accountRepo.FindAsync(User.Identity.GetUserName());
            account.QueueGive();
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task QueueReceiveAsync()
        {
            var account = await _accountRepo.FindAsync(User.Identity.GetUserName());
            account.QueueReceive();
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<TradeAccountVM> AccountStatusAsync()
        {
            var username = User.Identity.GetUserName();
            var account = await _accountRepo.Queryable()
                .Where(x => x.UserName == username)
                .Include(x => x.UserInfo).FirstAsync();
            var result = account.ToVM();
            return result;
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

        #endregion
    }
}