﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using BSE365.Model.Entities;
using BSE365.ViewModels;

namespace BSE365.Mappings
{
    public class MoneyTransactionVMMapping
    {
        public static Expression<Func<MoneyTransaction, MoneyTransactionVM.Giver>> GetExpToGiverVM()
        {
            Expression<Func<MoneyTransaction, MoneyTransactionVM.Giver>> result =
                x => new MoneyTransactionVM.Giver
                {
                    GiverId = x.GiverId,
                    ReceiverId = x.ReceiverId,
                    Created = x.Created,
                    LastModified = x.LastModified,
                    State = x.State,
                    AttachmentUrl = x.AttachmentUrl,
                    TransferedDate = x.TransferedDate,
                    ReceivedDate = x.ReceivedDate,
                    IsEnd = x.IsEnd,
                    DisplayName = x.Giver.UserInfo.DisplayName,
                    Email = x.Giver.UserInfo.Email,
                    PhoneNumber = x.Giver.UserInfo.PhoneNumber,
                    BankNumber = x.Giver.UserInfo.BankNumber,
                    BankName = x.Giver.UserInfo.BankName,
                    BankBranch = x.Giver.UserInfo.BankBranch
                };
            return result;
        }

        public static Expression<Func<MoneyTransaction, MoneyTransactionVM.Receiver>> GetExpToReceiverVM()
        {
            Expression<Func<MoneyTransaction, MoneyTransactionVM.Receiver>> result =
                x => new MoneyTransactionVM.Receiver
                {
                    GiverId = x.GiverId,
                    ReceiverId = x.ReceiverId,
                    Created = x.Created,
                    LastModified = x.LastModified,
                    State = x.State,
                    AttachmentUrl = x.AttachmentUrl,
                    TransferedDate = x.TransferedDate,
                    ReceivedDate = x.ReceivedDate,
                    IsEnd = x.IsEnd,
                    DisplayName = x.Receiver.UserInfo.DisplayName,
                    Email = x.Receiver.UserInfo.Email,
                    PhoneNumber = x.Receiver.UserInfo.PhoneNumber,
                    BankNumber = x.Receiver.UserInfo.BankNumber,
                    BankName = x.Receiver.UserInfo.BankName,
                    BankBranch = x.Receiver.UserInfo.BankBranch
                };
            return result;
        }
    }
}