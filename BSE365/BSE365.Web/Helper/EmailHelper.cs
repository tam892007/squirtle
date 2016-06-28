using BSE365.Common.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using Hangfire;

namespace BSE365.Helper
{
    public class EmailHelper
    {
        public static void SendLinkResetPassword(string code, string toEmail, string userName, string appUrl)
        {
            var contentBuilder = new StringBuilder();
            contentBuilder.Append("Click the below link to reset your password: <br/>");
            contentBuilder.AppendFormat("<a href='{0}/#/resetpassword?name={1}&code={2}&anonymous=true'>Link</a>",
                appUrl, userName, HttpUtility.UrlEncode(code));

            var mailMessage = new MailMessage
            {
                Subject = string.Format("{0} - You have requested to reset your password", userName),
                Body = contentBuilder.ToString(),
                IsBodyHtml = true,
            };

            mailMessage.To.Add(new MailAddress(toEmail));

            IMailClient mailClient = new SmtpMailClient();
            mailClient.Send(mailMessage);
        }

        public static void NotifyTransactionMapped(
            IList<string> giverIds, IList<string> giverEmails, string receiverId, string receiverEmail,
            string appUrl)
        {
            for (int i = 0; i < giverIds.Count; i++)
            {
                var giverId = giverIds[i];
                var giverEmail = giverEmails[i];
                BackgroundJob.Enqueue(() => NotifyGiverToGive(
                    giverId, giverEmail, receiverId, receiverEmail, appUrl));
            }
            BackgroundJob.Enqueue(() => NotifyReceiverToReceive(
                giverIds, giverEmails, receiverId, receiverEmail, appUrl));
        }

        private static void NotifyGiverToGive(
            string giverId, string giverEmail, string receiverId, string receiverEmail,
            string appUrl)
        {
            var subject = string.Format("{0} - Need to GIVE", giverId);

            var contentBuilder = new StringBuilder();
            contentBuilder.AppendFormat("Dear {0}, <br/>", giverId);
            contentBuilder.AppendFormat("You have been requested to give to <b>{0}</b>.<br/>", receiverId);

            contentBuilder.AppendFormat("Click the link below to login.<br/><a href='{0}'>Link</a>", appUrl);

            var mailMessage = new MailMessage
            {
                Subject = subject,
                Body = contentBuilder.ToString(),
                IsBodyHtml = true,
            };

            mailMessage.To.Add(new MailAddress(giverEmail));

            IMailClient mailClient = new SmtpMailClient();
            mailClient.Send(mailMessage);
        }

        private static void NotifyReceiverToReceive(
            IList<string> giverIds, IList<string> giverEmails, string receiverId, string receiverEmail,
            string appUrl)
        {
            var subject = string.Format("{0} - Request to RECEIVE has been processed", receiverId);

            var contentBuilder = new StringBuilder();
            contentBuilder.AppendFormat("Dear {0}, <br/>", receiverId);
            contentBuilder.AppendFormat("You will receive from following accounts: <br/>");
            foreach (var giverId in giverIds)
            {
                contentBuilder.AppendFormat("<b>{0}</b> <br/>", giverId);
            }

            contentBuilder.AppendFormat("Click the link below to login.<br/><a href='{0}'>Link</a>", appUrl);

            var mailMessage = new MailMessage
            {
                Subject = subject,
                Body = contentBuilder.ToString(),
                IsBodyHtml = true,
            };

            mailMessage.To.Add(new MailAddress(receiverEmail));

            IMailClient mailClient = new SmtpMailClient();
            mailClient.Send(mailMessage);
        }

        public static void NotifyTransactionGived(
            int transactionId, string giverId, string giverEmail, string receiverId, string receiverEmail,
            string appUrl)
        {
            var subject = string.Format("{0} - GOT money from {1} [Transaction #{2}]",
                receiverId, giverId, transactionId);

            var contentBuilder = new StringBuilder();
            contentBuilder.AppendFormat("Dear {0}, <br/>", receiverId);
            contentBuilder.AppendFormat(
                "<b>{0}</b> has transfered money to your account. Please confirm or report it if isn't legal.<br/>",
                giverId);

            contentBuilder.AppendFormat("Click the link below to login.<br/><a href='{0}'>Link</a>", appUrl);

            var mailMessage = new MailMessage
            {
                Subject = subject,
                Body = contentBuilder.ToString(),
                IsBodyHtml = true,
            };

            mailMessage.To.Add(new MailAddress(receiverEmail));

            IMailClient mailClient = new SmtpMailClient();
            mailClient.Send(mailMessage);
        }

        public static void NotifyTransactionNotTransfered(
            int transactionId, string giverId, string giverEmail, string receiverId, string receiverEmail,
            string newGiverId, string newGiverEmail)
        {
            IMailClient mailClient = new SmtpMailClient();
            {
                // for giver
                var subject = string.Format("{0} - Your account was locked [Transaction #{2}]",
                    giverId, receiverId, transactionId);

                var contentBuilder = new StringBuilder();
                contentBuilder.AppendFormat("Dear {0}, <br/>", giverId);
                contentBuilder.AppendFormat(
                    "You didn't transfer money to <b>{0}</b> in time. All of your accounts will be locked.<br/>",
                    receiverId);

                var mailMessage = new MailMessage
                {
                    Subject = subject,
                    Body = contentBuilder.ToString(),
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(new MailAddress(giverEmail));
                mailClient.Send(mailMessage);
            }
            {
                // for receiver
                var subject = string.Format("{0} - Transaction with {1} has been REPLACED [Transaction #{2}]",
                    receiverId, giverId, transactionId);

                var contentBuilder = new StringBuilder();
                contentBuilder.AppendFormat("Dear {0}, <br/>", giverId);
                contentBuilder.AppendFormat(
                    "Your transaction with <b>{0}</b> wasn't transferd in time, <b>{0}</b> has been locked. You will receive from <b>{1}</b>.<br/>",
                    giverId, newGiverId);

                var mailMessage = new MailMessage
                {
                    Subject = subject,
                    Body = contentBuilder.ToString(),
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(new MailAddress(receiverEmail));
                mailClient.Send(mailMessage);
            }
            {
                // for parent
                var subject = string.Format(
                    "{0} - You need transfer money to {1}.",
                    newGiverId, receiverId);

                var contentBuilder = new StringBuilder();
                contentBuilder.AppendFormat("Dear {0}, <br/>", newGiverId);
                contentBuilder.AppendFormat(
                    "Your branch {0} hasn't transferred money. All of his accounts have been locked.<br/>",
                    giverId);
                contentBuilder.AppendFormat(
                    "You need to transfer money to <b>{0}</b> to replace for <b>{1}</b>.<br/>",
                    receiverId, giverId);

                var mailMessage = new MailMessage
                {
                    Subject = subject,
                    Body = contentBuilder.ToString(),
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(new MailAddress(newGiverEmail));
                mailClient.Send(mailMessage);
            }
        }

        public static void NotifyTransactionReceived(
            int transactionId, string giverId, string giverEmail, string receiverId, string receiverEmail,
            string appUrl)
        {
            var subject = string.Format("{0} - Transaction with {1} has been COMPLETED [Transaction #{2}]",
                giverId, receiverId, transactionId);

            var contentBuilder = new StringBuilder();
            contentBuilder.AppendFormat("Dear {0}, <br/>", giverId);
            contentBuilder.AppendFormat("Your transaction with <b>{0}</b> has been completed successfully.<br/>",
                receiverId);

            contentBuilder.AppendFormat("Click the link below to login.<br/><a href='{0}'>Link</a>", appUrl);

            var mailMessage = new MailMessage
            {
                Subject = subject,
                Body = contentBuilder.ToString(),
                IsBodyHtml = true,
            };

            mailMessage.To.Add(new MailAddress(giverEmail));

            IMailClient mailClient = new SmtpMailClient();
            mailClient.Send(mailMessage);
        }

        public static void NotifyTransactionNotConfirmed(
            int transactionId, string giverId, string giverEmail, string receiverId, string receiverEmail)
        {
            IMailClient mailClient = new SmtpMailClient();
            {
                // for giver
                var subject = string.Format("{0} - Transaction with {1} has been COMPLETED [Transaction #{2}]",
                    giverId, receiverId, transactionId);

                var contentBuilder = new StringBuilder();
                contentBuilder.AppendFormat("Dear {0}, <br/>", giverId);
                contentBuilder.AppendFormat(
                    "<b>{0}</b> was not confirmed in time, your transaction has been completed.<br/>", receiverId);

                var mailMessage = new MailMessage
                {
                    Subject = subject,
                    Body = contentBuilder.ToString(),
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(new MailAddress(giverEmail));
                mailClient.Send(mailMessage);
            }
            {
                // for receiver
                var subject =
                    string.Format("{0} - Your account was locked [Transaction #{2}]",
                        receiverId, giverId, transactionId);

                var contentBuilder = new StringBuilder();
                contentBuilder.AppendFormat("Dear {0}, <br/>", giverId);
                contentBuilder.AppendFormat(
                    "Your transaction with <b>{0}</b> wasn't confirmed in time. All of your accounts will be locked.<br/>",
                    giverId);

                var mailMessage = new MailMessage
                {
                    Subject = subject,
                    Body = contentBuilder.ToString(),
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(new MailAddress(receiverEmail));
                mailClient.Send(mailMessage);
            }
        }
    }
}