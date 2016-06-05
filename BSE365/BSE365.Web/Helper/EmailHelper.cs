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
    }
}