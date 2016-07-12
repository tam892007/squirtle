using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BSE365.Base.Infrastructures;
using BSE365.Base.Repositories.Contracts;
using BSE365.Base.UnitOfWork;
using BSE365.Base.UnitOfWork.Contracts;
using BSE365.Common.Constants;
using BSE365.Model.Entities;
using BSE365.Model.Enum;
using BSE365.Repository.DataContext;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace BSE365.Hub
{
    [Authorize]
    [HubName("Message")]
    public class MessageHub : Microsoft.AspNet.SignalR.Hub
    {
        private readonly IUnitOfWork _unitOfWork;
        private IRepository<Message> _repository;

        public MessageHub()
        {
            _unitOfWork = new UnitOfWork(new BSE365Context());
            _repository = _unitOfWork.Repository<Message>();
        }

        private string CurrentAccount
        {
            get { return Context.User.Identity.GetUserName(); }
        }

        public string GetCurrentAccount()
        {
            return CurrentAccount;
        }

        public bool ValidAccount(string targetAccount)
        {
            var result = _unitOfWork.Repository<Account>().Queryable().Any(x => x.UserName == targetAccount);
            return result;
        }

        /// <summary>
        /// Get UnRead Messages
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Message> GetUnreadMessages()
        {
            var query = _repository.Queryable()
                .Where(x => x.Type == MessageType.Message && x.State == MessageState.UnRead &&
                            x.ToAccount == CurrentAccount)
                .OrderByDescending(x => x.Created);
            var result = query.ToArray();
            return result;
        }

        /// <summary>
        /// Get Message History with Account
        /// </summary>
        /// <param name="targetAccount"></param>
        /// <param name="pointTime"></param>
        /// <returns></returns>
        public IEnumerable<Message> GetMessageHistoryWith(string targetAccount, DateTime pointTime)
        {
            var query = _repository.Queryable()
                .Where(x => x.Type == MessageType.Message && x.State == MessageState.UnRead &&
                            x.Created < pointTime &&
                            ((x.FromAccount == targetAccount && x.ToAccount == CurrentAccount) ||
                             (x.FromAccount == CurrentAccount && x.ToAccount == targetAccount)))
                .OrderByDescending(x => x.Created);
            var result = query.ToArray();
            return result;
        }

        /// <summary>
        /// Send Message to Account
        /// </summary>
        /// <param name="targetAccount"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Message SendMessageTo(string targetAccount, string message)
        {
            var msg = new Message
            {
                Id = Guid.NewGuid().ToString(),
                Created = DateTime.Now,
                FromAccount = CurrentAccount,
                ToAccount = targetAccount,
                Messsage = message,
                Type = MessageType.Message,
                State = MessageState.UnRead,
                ObjectState = ObjectState.Added,
            };
            _repository.Insert(msg);
            _unitOfWork.SaveChanges();
            Clients.User(targetAccount).message(msg);
            return msg;
        }

        /// <summary>
        /// Update Message State
        /// </summary>
        /// <param name="messageIds"></param>
        public void UpdateMessageStates(string[] messageIds)
        {
            var messages = _repository.Queryable()
                .Where(x => messageIds.Contains(x.Id)).ToList();
            foreach (var message in messages)
            {
                message.State = MessageState.Readed;
                message.ObjectState = ObjectState.Modified;
            }
            _unitOfWork.SaveChanges();
        }


        /// <summary>
        /// Get UnRead Notifications
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Message> GetUnreadNotifications()
        {
            var query = _repository.Queryable()
                .Where(x => x.Type == MessageType.Notification && x.State == MessageState.UnRead &&
                            x.ToAccount == CurrentAccount)
                .OrderByDescending(x => x.Created);
            var result = query.ToArray();
            return result;
        }

        /// <summary>
        /// Send Notification to Account
        /// </summary>
        /// <param name="targetAccount"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Message SendNotificationTo(string targetAccount, string message)
        {
            var notification = new Message
            {
                Id = Guid.NewGuid().ToString(),
                Created = DateTime.Now,
                FromAccount = SystemAdmin.UserName,
                ToAccount = targetAccount,
                Messsage = message,
                Type = MessageType.Message,
                State = MessageState.UnRead,
                ObjectState = ObjectState.Added,
            };
            _repository.Insert(notification);
            _unitOfWork.SaveChanges();
            Clients.User(targetAccount).notify(notification);
            return notification;
        }

        /// <summary>
        /// Update Notification State
        /// </summary>
        /// <param name="notificationIds"></param>
        public void UpdateNotificationStates(string[] notificationIds)
        {
            var notifications = _repository.Queryable()
                .Where(x => notificationIds.Contains(x.Id)).ToList();
            foreach (var notification in notifications)
            {
                notification.State = MessageState.Readed;
                notification.ObjectState = ObjectState.Modified;
            }
            _unitOfWork.SaveChanges();
        }
    }
}