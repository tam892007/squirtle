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
    [Authorize]
    [RoutePrefix("api/Message")]
    public class MessageController : ApiController
    {
        IUnitOfWorkAsync _unitOfWork;
        IRepositoryAsync<Message> _messageRepo;

        public MessageController(
            IUnitOfWorkAsync unitOfWork,
            IRepositoryAsync<Message> messageRepo)
        {
            _unitOfWork = unitOfWork;
            _messageRepo = messageRepo;
        }

        [HttpGet]
        [Route("MessageSent")]
        public async Task<IHttpActionResult> MessageSent()
        {
            var result = await MessageSentAsync();
            return Ok(result);
        }

        [HttpGet]
        [Route("MessageReceived")]
        public async Task<IHttpActionResult> MessageReceived()
        {
            var result = await MessageReceivedAsync();
            return Ok(result);
        }

        #region internal method

        private async Task<IEnumerable<MessageViewModel>> MessageSentAsync()
        {
            var username = User.Identity.GetUserName();
            var data = await _messageRepo.Queryable().Where(x =>
                x.FromId == username).ToListAsync();
            return data.Select(x => x.ToViewModel());
        }

        private async Task<IEnumerable<MessageViewModel>> MessageReceivedAsync()
        {
            var username = User.Identity.GetUserName();
            var data = await _messageRepo.Queryable().Where(x =>
                x.ToId == username).ToListAsync();
            return data.Select(x => x.ToViewModel());
        }

        #endregion
    }
}