using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using BSE365.Model.Entities;
using BSE365.Model.Enum;
using BSE365.ViewModels;

namespace BSE365.Mappings
{
    public static class TransactionHistoryVMMapping
    {
        public static Expression<Func<IGrouping<int, MoneyTransaction>, TransactionHistoryVM>> GetExpToVM(
            string username)
        {
            Expression<Func<IGrouping<int, MoneyTransaction>, TransactionHistoryVM>> result =
                x => new TransactionHistoryVM
                {
                    Id = x.Key,
                    Type = x.Select(t => t.GiverId == username
                        ? AccountState.InGiveTransaction
                        : AccountState.InReceiveTransaction).FirstOrDefault(),
                    BeginDate = x.Min(t => t.Created),
                    LastModifiedDate = x.Max(t => t.LastModified),
                    NumOfSuccesses = x.Count(t => t.IsEnd),
                    NumOfTransactions = x.Count(),
                };
            return result;
        }
    }
}