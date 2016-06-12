using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using BSE365.Model.Entities;
using BSE365.ViewModels;

namespace BSE365.Mappings
{
    public static class MoneyTransactionVMMapping
    {
        public static Expression<Func<MoneyTransaction, MoneyTransactionVM.Giver>> GetExpToGiverVM()
        {
            Expression<Func<MoneyTransaction, MoneyTransactionVM.Giver>> result =
                x => new MoneyTransactionVM.Giver
                {
                    Id = x.Id,
                    GiverId = x.GiverId,
                    ReceiverId = x.ReceiverId,
                    Created = x.Created,
                    LastModified = x.LastModified,
                    Type = x.Type,
                    State = x.State,
                    AttachmentUrl = x.AttachmentUrl,
                    TransferedDate = x.TransferedDate,
                    ReceivedDate = x.ReceivedDate,
                    WaitingGiverId = x.WaitingGiverId,
                    WaitingReceiverId = x.WaitingReceiverId,
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
                    Id = x.Id,
                    GiverId = x.GiverId,
                    ReceiverId = x.ReceiverId,
                    Created = x.Created,
                    LastModified = x.LastModified,
                    Type = x.Type,
                    State = x.State,
                    AttachmentUrl = x.AttachmentUrl,
                    TransferedDate = x.TransferedDate,
                    ReceivedDate = x.ReceivedDate,
                    WaitingGiverId = x.WaitingGiverId,
                    WaitingReceiverId = x.WaitingReceiverId,
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

        public static Expression<Func<MoneyTransaction, MoneyTransactionVM.Punishment>> GetExpToPunismentVM()
        {
            Expression<Func<MoneyTransaction, MoneyTransactionVM.Punishment>> result =
                x => new MoneyTransactionVM.Punishment
                {
                    Id = x.Id,
                    GiverId = x.GiverId,
                    ReceiverId = x.ReceiverId,
                    Created = x.Created,
                    LastModified = x.LastModified,
                    Type = x.Type,
                    State = x.State,
                    AttachmentUrl = x.AttachmentUrl,
                    TransferedDate = x.TransferedDate,
                    ReceivedDate = x.ReceivedDate,
                    WaitingGiverId = x.WaitingGiverId,
                    WaitingReceiverId = x.WaitingReceiverId,
                    IsEnd = x.IsEnd,
                    DisplayName = x.Receiver.UserInfo.DisplayName,
                    Email = x.Receiver.UserInfo.Email,
                    PhoneNumber = x.Receiver.UserInfo.PhoneNumber,
                    BankNumber = x.Receiver.UserInfo.BankNumber,
                    BankName = x.Receiver.UserInfo.BankName,
                    BankBranch = x.Receiver.UserInfo.BankBranch,
                    RelatedTransactionId = x.RelatedTransactionId,
                    ForAccount = x.RelatedTransaction.GiverId,
                    ForUser = x.RelatedTransaction.Giver.UserInfo.DisplayName,
                };
            return result;
        }

        public static T UpdateVm<T>(this MoneyTransaction model, T vm)
            where T : MoneyTransactionVM.Simple
        {
            vm.Created = model.Created;
            vm.LastModified = model.LastModified;
            vm.Type = model.Type;
            vm.State = model.State;
            vm.AttachmentUrl = model.AttachmentUrl;
            vm.TransferedDate = model.TransferedDate;
            vm.ReceivedDate = model.ReceivedDate;
            vm.WaitingGiverId = model.WaitingGiverId;
            vm.WaitingReceiverId = model.WaitingReceiverId;
            vm.IsEnd = model.IsEnd;
            return vm;
        }


        public static Expression<Func<MoneyTransaction, MoneyTransactionVM.Base>> GetExpToVM()
        {
            Expression<Func<MoneyTransaction, MoneyTransactionVM.Base>> result =
                x => new MoneyTransactionVM.Base
                {
                    Id = x.Id,
                    GiverId = x.GiverId,
                    ReceiverId = x.ReceiverId,
                    Created = x.Created,
                    LastModified = x.LastModified,
                    Type = x.Type,
                    State = x.State,
                    AttachmentUrl = x.AttachmentUrl,
                    TransferedDate = x.TransferedDate,
                    ReceivedDate = x.ReceivedDate,
                    WaitingGiverId = x.WaitingGiverId,
                    WaitingReceiverId = x.WaitingReceiverId,
                    IsEnd = x.IsEnd,
                    GiverDisplayName = x.Giver.UserInfo.DisplayName,
                    GiverEmail = x.Giver.UserInfo.Email,
                    GiverPhoneNumber = x.Giver.UserInfo.PhoneNumber,
                    GiverBankNumber = x.Giver.UserInfo.BankNumber,
                    GiverBankName = x.Giver.UserInfo.BankName,
                    GiverBankBranch = x.Giver.UserInfo.BankBranch,
                    ReceiverDisplayName = x.Receiver.UserInfo.DisplayName,
                    ReceiverEmail = x.Receiver.UserInfo.Email,
                    ReceiverPhoneNumber = x.Receiver.UserInfo.PhoneNumber,
                    ReceiverBankNumber = x.Receiver.UserInfo.BankNumber,
                    ReceiverBankName = x.Receiver.UserInfo.BankName,
                    ReceiverBankBranch = x.Receiver.UserInfo.BankBranch
                };
            return result;
        }
    }
}