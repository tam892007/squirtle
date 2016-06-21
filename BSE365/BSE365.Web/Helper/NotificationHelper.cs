using BSE365.Common.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using BSE365.Repository.DataContext;

namespace BSE365.Helper
{
    public class NotificationHelper
    {
        public static void NotifyTransactionMapped(IList<string> giverIds, string receiverId)
        {
            using (var dataContext = new BSE365Context())
            {
                dataContext.SaveChanges();
            }
        }

        public static void NotifyTransactionGived(
            int transactionId, string giverId, string receiverId)
        {
            using (var dataContext = new BSE365Context())
            {
                dataContext.SaveChanges();
            }
        }

        public static void NotifyTransactionReceived(int transactionId, string giverId, string receiverId)
        {
            using (var dataContext = new BSE365Context())
            {
                dataContext.SaveChanges();
            }
        }

        public static void NotifyTransactionNotTransfered(int transactionId, string giverId, string receiverId,
            string newGiverId)
        {
            using (var dataContext = new BSE365Context())
            {
                dataContext.SaveChanges();
            }
        }

        public static void NotifyTransactionNotConfirmed(int transactionId, string giverId, string receiverId)
        {
            using (var dataContext = new BSE365Context())
            {
                dataContext.SaveChanges();
            }
        }
    }
}