using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Http;
using BSE365.Base.Infrastructures;
using BSE365.Base.Repositories.Contracts;
using BSE365.Base.UnitOfWork.Contracts;
using BSE365.Common.Constants;
using BSE365.Common.Helper;
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
using LinqKit;

namespace BSE365.Api
{
    [Authorize]
    [RoutePrefix("api/userinfo")]
    public class UserInfoController : ApiController
    {
        private IUnitOfWorkAsync _unitOfWork;
        private IRepositoryAsync<UserInfo> _infoRepo;

        private UserProfileRepository _userRepo;

        public UserInfoController(
            IUnitOfWorkAsync unitOfWork,
            IRepositoryAsync<UserInfo> infoRepo)
        {
            _unitOfWork = unitOfWork;
            _infoRepo = infoRepo;

            _userRepo = new UserProfileRepository();
        }

        [Authorize(Roles = UserRolesText.SuperAdmin
                           + "," + UserRolesText.ManageUserInfo + "," + UserRolesText.MapTransaction + "," +
                           UserRolesText.ManageTransaction)]
        [HttpPost]
        [Route("QueryInfo")]
        public async Task<IHttpActionResult> QueryInfo(FilterVM filter)
        {
            var result = await QueryInfoAsync(filter);
            return Ok(result);
        }


        [Authorize(Roles = UserRolesText.SuperAdmin
                           + "," + UserRolesText.ManageUserInfo + "," + UserRolesText.MapTransaction + "," +
                           UserRolesText.ManageTransaction)]
        [HttpPost, HttpGet]
        [Route("WaitingInfomations")]
        public async Task<IHttpActionResult> WaitingInfomations(string userPrefix)
        {
            var givers = await WaitingGiverInfomationsAsync(userPrefix);
            var receivers = await WaitingReceiverInfomationsAsync(userPrefix);
            return Ok(new {GiverData = givers, ReceiverData = receivers});
        }

        [Authorize(Roles = UserRolesText.SuperAdmin
                           + "," + UserRolesText.ManageUserInfo + "," + UserRolesText.MapTransaction + "," +
                           UserRolesText.ManageTransaction)]
        [HttpPost]
        [Route("QueryTransaction")]
        public async Task<IHttpActionResult> QueryTransaction(FilterVM filter)
        {
            var result = await QueryTransactionAsync(filter);
            return Ok(result);
        }

        #region internal method

        private async Task<PageViewModel<TradeUserInfoVM>> QueryInfoAsync(FilterVM filter)
        {
            int totalPageCount;

            Expression<Func<UserInfo, bool>> predicate = null;

            if (filter.Search.PredicateObject != null)
            {
                var summary = filter.Search.PredicateObject.Value<string>("$");
                if (!string.IsNullOrWhiteSpace(summary))
                {
                    summary = summary.ToUpper();
                    Expression<Func<UserInfo, bool>> byUserPrefix = x => x.UserPrefix == summary;
                    Expression<Func<UserInfo, bool>> byBankNumber = x => x.BankNumber.Contains(summary);
                    /*Expression<Func<UserInfo, bool>> byDisplayName = x => x.DisplayName.ToUpper().Contains(summary);*/

                    predicate = byUserPrefix.Or(byBankNumber) /*.Or(byDisplayName)*/;
                }
            }

            var selectExpression = TradeUserInfoVMMapping.GetExpToVM();
            var data = await (predicate == null ? _infoRepo.Query() : _infoRepo.Query(predicate))
                .OrderBy(x => x.OrderByDescending(a => a.UserPrefix))
                .SelectQueryable(selectExpression,
                    filter.Pagination.Start/filter.Pagination.Number + 1, filter.Pagination.Number, out totalPageCount)
                .ToListAsync();

            var page = new PageViewModel<TradeUserInfoVM>
            {
                Data = data,
                TotalItems = totalPageCount
            };

            return page;
        }

        private async Task<List<WaitingAccountVM>> WaitingGiverInfomationsAsync(string userPrefix)
        {
            var expression = WaitingAccountVMMapping.GetExpToSimpleGiverVM();
            var result = await _unitOfWork.RepositoryAsync<WaitingGiver>().Queryable()
                .Where(x => x.AccountId.Contains(userPrefix)).Select(expression).ToListAsync();
            return result;
        }

        private async Task<List<WaitingAccountVM>> WaitingReceiverInfomationsAsync(string userPrefix)
        {
            var expression = WaitingAccountVMMapping.GetExpToSimpleReceiverVM();
            var result = await _unitOfWork.RepositoryAsync<WaitingReceiver>().Queryable()
                .Where(x => x.AccountId.Contains(userPrefix)).Select(expression).ToListAsync();
            return result;
        }

        private async Task<PageViewModel<MoneyTransactionVM.Base>> QueryTransactionAsync(FilterVM filter)
        {
            int totalPageCount;
            var transactionRepo = _unitOfWork.RepositoryAsync<MoneyTransaction>();
            var summary = filter.Search.PredicateObject.Value<string>("$");
            var userPrefix = filter.Search.PredicateObject.Value<string>("userPrefix");
            var usernames = Utilities.GetRangeUserName(userPrefix);
            Expression<Func<MoneyTransaction, bool>> byGiver = x => usernames.Contains(x.GiverId);
            Expression<Func<MoneyTransaction, bool>> byReceiver = x => usernames.Contains(x.ReceiverId);
            var predicate = byGiver.Or(byReceiver);
            if (!string.IsNullOrWhiteSpace(summary))
            {
                int id;
                if (int.TryParse(summary, out id))
                {
                    Expression<Func<MoneyTransaction, bool>> byId = x => x.Id == id;
                    predicate = byId.And(predicate);
                }
            }
            IQueryFluent<MoneyTransaction> query = transactionRepo.Query(predicate);

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

        #endregion
    }
}