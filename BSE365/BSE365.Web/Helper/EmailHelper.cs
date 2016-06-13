using BSE365.Common.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace BSE365.Helper
{
    public class EmailHelper
    {
        public static void SendLinkResetPassword(string code, string toEmail, string userName, string appUrl)
        {
            var contentBuilder = new StringBuilder();
            contentBuilder.Append("Click the below link to reset your password: <br/>");
            contentBuilder.AppendFormat("<a href='{0}/#/resetpassword?name={1}&code={2}&anonymous=true'>Link</a>", appUrl, userName, HttpUtility.UrlEncode(code));

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

        public static void SendNotificationToGive(string giverEmail, string giverId, string receiverId, string appUrl)
        {
            var contentBuilder = new StringBuilder();
            contentBuilder.AppendFormat("Dear {0}, <br/>You have been requested to give to <b>{1}</b>. <br/>Click the link below to login.<br/>", giverId, receiverId);
            contentBuilder.AppendFormat("<a href='{0}'>Link</a>", appUrl);

            var mailMessage = new MailMessage
            {
                Subject = string.Format("{0} - Need to GIVE", giverId),
                Body = contentBuilder.ToString(),
                IsBodyHtml = true,
            };

            mailMessage.To.Add(new MailAddress(giverEmail));

            IMailClient mailClient = new SmtpMailClient();
            mailClient.Send(mailMessage);
        }

        public static void SendNotificationToReceive(string receiverEmail, IList<string> giverIds, string receiverId, string appUrl)
        {
            var contentBuilder = new StringBuilder();
            contentBuilder.AppendFormat("Dear {0}, <br/>You will receive from following accounts: <br/>", receiverId);
            foreach (var giverId in giverIds)
            {
                contentBuilder.AppendFormat("<b>{0}</b> <br/>", giverId);
            }

            contentBuilder.AppendFormat("Click the link below to login.<br/> <a href='{0}'>Link</a>", appUrl);

            var mailMessage = new MailMessage
            {
                Subject = string.Format("{0} - Request to RECEIVE has been processed", receiverId),
                Body = contentBuilder.ToString(),
                IsBodyHtml = true,
            };

            mailMessage.To.Add(new MailAddress(receiverEmail));

            IMailClient mailClient = new SmtpMailClient();
            mailClient.Send(mailMessage);
        }

        public static void SendNotificationBeingGived(string receiverEmail, string giverId, string receiverId, string appUrl)
        {
            var contentBuilder = new StringBuilder();
            contentBuilder.AppendFormat("Dear {0}, <br/><b>{1}</b> has transfered money to your account. Please confirm or report it if isnt legal.<br/>Click the link below to login.<br/>",
                receiverId, giverId);
            contentBuilder.AppendFormat("<a href='{0}'>Link</a>", appUrl);

            var mailMessage = new MailMessage
            {
                Subject = string.Format("{0} - GOT money from {1}", receiverId, giverId),
                Body = contentBuilder.ToString(),
                IsBodyHtml = true,
            };

            mailMessage.To.Add(new MailAddress(receiverEmail));

            IMailClient mailClient = new SmtpMailClient();
            mailClient.Send(mailMessage);
        }

        public static void SendNotificationConfirmReceived(string giverEmail, string giverId, string receiverId, string appUrl)
        {
            var contentBuilder = new StringBuilder();
            contentBuilder.AppendFormat("Dear {0}, <br/>Your transaction with {1} has been completed successfully.<br/>Click the link below to login.<br/>",
                giverId, receiverId);
            contentBuilder.AppendFormat("<a href='{0}'>Link</a>", appUrl);

            var mailMessage = new MailMessage
            {
                Subject = string.Format("{0} - Transaction with {1} has been COMPLETED", giverId, receiverId),
                Body = contentBuilder.ToString(),
                IsBodyHtml = true,
            };

            mailMessage.To.Add(new MailAddress(giverEmail));

            IMailClient mailClient = new SmtpMailClient();
            mailClient.Send(mailMessage);
        }
    }
}