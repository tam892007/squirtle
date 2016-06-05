using System.Net.Mail;

namespace BSE365.Common.Email
{
    public interface IMailClient
    {
        void SendMessage(string[] to, string subject, string message, string[] cc = null);
        void Send(MailMessage message);
    }
}
