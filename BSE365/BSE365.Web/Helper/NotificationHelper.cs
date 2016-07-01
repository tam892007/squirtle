using BSE365.Common.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using BSE365.Base.Infrastructures;
using BSE365.Common.Constants;
using BSE365.Hub;
using BSE365.Model.Entities;
using BSE365.Model.Enum;
using BSE365.Repository.DataContext;
using Microsoft.AspNet.SignalR;

namespace BSE365.Helper
{
    public class NotificationHelper
    {
        public static void NotifyTransactionMapped(IList<string> giverIds, string receiverId)
        {
            using (var dataContext = new BSE365Context())
            {
                for (int i = 0; i < giverIds.Count; i++)
                {
                    var giverId = giverIds[i];
                    var contentBuilder = new StringBuilder();
                    contentBuilder.AppendFormat("Dear {0}, <br/>", giverId);
                    contentBuilder.AppendFormat("You have been requested to give to <b>{0}</b>.<br/>", receiverId);

                    var message = contentBuilder.ToString();
                    SendNotificationTo(giverId, message, dataContext);
                }
                {
                    // for receiver
                    var contentBuilder = new StringBuilder();
                    contentBuilder.AppendFormat("Dear {0}, <br/>", receiverId);
                    contentBuilder.AppendFormat("You will receive from following accounts: <br/>");
                    foreach (var giverId in giverIds)
                    {
                        contentBuilder.AppendFormat("<b>{0}</b> <br/>", giverId);
                    }

                    var message = contentBuilder.ToString();
                    SendNotificationTo(receiverId, message, dataContext);
                }
                dataContext.SaveChanges();
            }
        }

        public static void NotifyTransactionGived(
            int transactionId, string giverId, string receiverId)
        {
            using (var dataContext = new BSE365Context())
            {
                var contentBuilder = new StringBuilder();
                contentBuilder.AppendFormat("Dear {0}, <br/>", receiverId);
                contentBuilder.AppendFormat(
                    "<b>{0}</b> has transfered money to your account. Please confirm or report it if isn't legal.<br/>",
                    giverId);

                var message = contentBuilder.ToString();
                SendNotificationTo(receiverId, message, dataContext);
                dataContext.SaveChanges();
            }
        }

        public static void NotifyTransactionNotTransfered(int transactionId, string giverId, string receiverId,
            string newGiverId)
        {
            using (var dataContext = new BSE365Context())
            {
                {
                    // for giver
                    var contentBuilder = new StringBuilder();
                    contentBuilder.AppendFormat("Dear {0}, <br/>", giverId);
                    contentBuilder.AppendFormat(
                        "You didn't transfer money to <b>{0}</b> in time. All of your accounts will be locked.<br/>",
                        receiverId);

                    var message = contentBuilder.ToString();
                    SendNotificationTo("", message, dataContext);
                }
                {
                    // for receiver
                    var contentBuilder = new StringBuilder();
                    contentBuilder.AppendFormat("Dear {0}, <br/>", giverId);
                    contentBuilder.AppendFormat(
                        "Your transaction with <b>{0}</b> wasn't transferd in time, <b>{0}</b> has been locked. You will receive from <b>{1}</b>.<br/>",
                        giverId, newGiverId);

                    var message = contentBuilder.ToString();
                    SendNotificationTo("", message, dataContext);
                }
                {
                    // for parent
                    var contentBuilder = new StringBuilder();
                    contentBuilder.AppendFormat("Dear {0}, <br/>", newGiverId);
                    contentBuilder.AppendFormat(
                        "Your branch {0} hasn't transferred money. All of his accounts have been locked.<br/>",
                        giverId);
                    contentBuilder.AppendFormat(
                        "You need to transfer money to <b>{0}</b> to replace for <b>{1}</b>.<br/>",
                        receiverId, giverId);

                    var message = contentBuilder.ToString();
                    SendNotificationTo("", message, dataContext);
                }

                dataContext.SaveChanges();
            }
        }

        public static void NotifyTransactionReceived(int transactionId, string giverId, string receiverId)
        {
            using (var dataContext = new BSE365Context())
            {
                var contentBuilder = new StringBuilder();
                contentBuilder.AppendFormat("Dear {0}, <br/>", giverId);
                contentBuilder.AppendFormat("Your transaction with <b>{0}</b> has been completed successfully.<br/>",
                    receiverId);

                var message = contentBuilder.ToString();
                SendNotificationTo(giverId, message, dataContext);
                dataContext.SaveChanges();
            }
        }

        public static void NotifyTransactionNotConfirmed(int transactionId, string giverId, string receiverId)
        {
            using (var dataContext = new BSE365Context())
            {
                {
                    // for giver
                    var contentBuilder = new StringBuilder();
                    contentBuilder.AppendFormat("Dear {0}, <br/>", giverId);
                    contentBuilder.AppendFormat(
                        "<b>{0}</b> was not confirmed in time, your transaction has been completed.<br/>", receiverId);

                    var message = contentBuilder.ToString();
                    SendNotificationTo(giverId, message, dataContext);
                }
                {
                    // for receiver
                    var contentBuilder = new StringBuilder();
                    contentBuilder.AppendFormat("Dear {0}, <br/>", receiverId);
                    contentBuilder.AppendFormat(
                        "Your transaction with <b>{0}</b> wasn't confirmed in time. All of your accounts will be locked.<br/>",
                        giverId);

                    var message = contentBuilder.ToString();
                    SendNotificationTo(receiverId, message, dataContext);
                }
                dataContext.SaveChanges();
            }
        }

        private static Message SendNotificationTo(string targetAccount, string message, BSE365Context dataContext)
        {
            var notification = new Message
            {
                Id = Guid.NewGuid().ToString(),
                Created = DateTime.Now,
                FromAccount = SystemAdmin.UserName,
                ToAccount = targetAccount,
                Messsage = message,
                Type = MessageType.Notification,
                State = MessageState.UnRead,
                ObjectState = ObjectState.Added,
            };
            dataContext.Messages.Add(notification);
            var notificationHub = GlobalHost.ConnectionManager.GetHubContext<MessageHub>();
            notificationHub.Clients.User(targetAccount).notify(notification);
            return notification;
        }
    }
}