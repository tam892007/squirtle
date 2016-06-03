using BSE365.Common;
using BSE365.Common.Helper;
using BSE365.Model.Entities;
using BSE365.Repository.DataContext;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using BSE365.Model.Enum;
using BSE365.Base.Infrastructures;

namespace BSE365.Repository.Repositories
{
    public class MessageRepository : IDisposable
    {
        private BSE365Context _ctx;

        public MessageRepository()
        {
            _ctx = new BSE365Context();
        }

        public async Task<int> Send(Message message)
        {
            message.Status = MessageState.UnRead;
            message.SendTime = DateTime.Now;
            message.ObjectState = ObjectState.Added;
            _ctx.Messages.Add(message);
            return await _ctx.SaveChangesAsync();
        }

        public void Dispose()
        {
            _ctx.Dispose();
        }
    }
}
