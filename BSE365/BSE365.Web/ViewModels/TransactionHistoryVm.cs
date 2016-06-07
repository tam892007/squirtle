using System;
using BSE365.Model.Enum;

namespace BSE365.ViewModels
{
    public class TransactionHistoryVM
    {
        public int Id { get; set; }

        public string AccountId { get; set; }

        public AccountState Type { get; set; }

        public int NumOfTransactions { get; set; }

        public int NumOfSuccesses { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime LastModifiedDate { get; set; }
    }
}