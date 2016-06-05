using System.Net.Mail;

namespace BSE365.Common.Email
{
    public class SmtpMailClient : MailClientBase
    {
        public override void Send(MailMessage message)
        {
            var smtpClient = new SmtpClient();
            smtpClient.Send(message);
        }
    }
}
