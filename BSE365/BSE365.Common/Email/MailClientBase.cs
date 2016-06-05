using System;
using System.Net.Mail;

namespace BSE365.Common.Email
{
    /// <summary>
    /// The base class for mail clients
    /// </summary>
    public abstract class MailClientBase : IMailClient
    {
        /// <summary>
        /// Creates the MailMessage
        /// </summary>
        /// <param name="to">The recipients.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        /// <param name="cc">The cc.</param>
        /// <returns></returns>
        protected virtual MailMessage CreateMailMessage(string[] to, string subject, string message, string[] cc = null)
        {
            var mailMessage = new MailMessage
            {
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };
            foreach (var recipient in to)
            {
                if (!String.IsNullOrWhiteSpace(recipient))
                    mailMessage.To.Add(new MailAddress(recipient));
            }

            if (cc != null && cc.Length > 0)
                foreach (var ccRecipient in cc)
                {
                    if (!String.IsNullOrWhiteSpace(ccRecipient))
                        mailMessage.CC.Add(new MailAddress(ccRecipient));
                }

            return mailMessage;
        }

        /// <summary>
        /// Send the message
        /// </summary>
        /// <param name="to">The recipients</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        /// <param name="cc">The cc.</param>
        public void SendMessage(string[] to, string subject, string message, string[] cc = null)
        {
            MailMessage mailMessage = CreateMailMessage(to, subject, message, cc);
            if (mailMessage.To.Count > 0)
                Send(mailMessage);
        }

        /// <summary>
        /// Actually sends the specified message.
        /// </summary>
        public abstract void Send(MailMessage message);
    }
}
