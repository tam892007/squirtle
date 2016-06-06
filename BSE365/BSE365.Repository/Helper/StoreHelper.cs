using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSE365.Base.Infrastructures;
using BSE365.Model.Entities;
using BSE365.Model.Enum;
using BSE365.Repository.DataContext;

namespace BSE365.Repository.Helper
{
    public class StoreHelper
    {
        public static void MapWaitingGiversAndWaitingReceivers()
        {
            var now = DateTime.Now;
            using (var context = new BSE365Context())
            {
                var queuedWaitingGivers =
                    context.WaitingGivers.Include(x => x.Account)
                        .OrderByDescending(x => x.Priority)
                        .ThenBy(x => x.Created)
                        .ToList();
                var queuedWaitingReceivers =
                    context.WaitingReceivers.Include(x => x.Account)
                        .OrderByDescending(x => x.Priority)
                        .ThenBy(x => x.Created)
                        .ToList();
                var transformedWaitingGivers = new List<WaitingBase>();
                var transformedWaitingReceivers = new List<WaitingBase>();
                foreach (var queued in queuedWaitingGivers)
                {
                    for (int i = 0; i < queued.Amount; i++)
                    {
                        transformedWaitingGivers.Add(new WaitingBase
                        {
                            Id = queued.Id,
                            Priority = queued.Priority,
                            AccountId = queued.AccountId,
                            Account = queued.Account,
                            Created = queued.Created,
                            Amount = 1
                        });
                    }
                }
                foreach (var queued in queuedWaitingReceivers)
                {
                    for (int i = 0; i < queued.Amount; i++)
                    {
                        transformedWaitingReceivers.Add(new WaitingBase
                        {
                            Id = queued.Id,
                            Priority = queued.Priority,
                            AccountId = queued.AccountId,
                            Account = queued.Account,
                            Created = queued.Created,
                            Amount = 1
                        });
                    }
                }

                var mappedTransaction = new List<MoneyTransaction>();

                while (transformedWaitingGivers.Count > 0 && transformedWaitingReceivers.Count > 0)
                {
                    var giver = transformedWaitingGivers[0];
                    transformedWaitingGivers.RemoveAt(0);

                    var receiverIndex = 0;
                    var receiver = transformedWaitingReceivers[receiverIndex ++];

                    var isExistSamGiverAndReceiver = mappedTransaction.Any(
                        x => x.GiverId == giver.AccountId && x.ReceiverId == receiver.AccountId);
                    while (isExistSamGiverAndReceiver && receiverIndex < transformedWaitingReceivers.Count)
                    {
                        receiver = transformedWaitingReceivers[receiverIndex ++];

                        isExistSamGiverAndReceiver = mappedTransaction.Any(
                            x => x.GiverId == giver.AccountId && x.ReceiverId == receiver.AccountId);
                    }

                    if (receiverIndex == transformedWaitingReceivers.Count)
                    {
                        break;
                    }

                    receiver = transformedWaitingReceivers[receiverIndex - 1];
                    transformedWaitingReceivers.RemoveAt(receiverIndex - 1);


                    var queuedGiver = queuedWaitingGivers.First(x => x.Id == giver.Id);
                    var queuedReceiver = queuedWaitingReceivers.First(x => x.Id == receiver.Id);

                    var queuedGiverOldState = queuedGiver.Account.State;
                    var queuedReceiverOldState = queuedReceiver.Account.State;

                    var transaction = new MoneyTransaction
                    {
                        GiverId = giver.AccountId,
                        ReceiverId = receiver.AccountId,
                        Created = now,
                        LastModified = now,
                        WaitingGiverId = queuedGiver.Id,
                        WaitingReceiverId = queuedReceiver.Id,
                        ObjectState = ObjectState.Added,
                    };

                    mappedTransaction.Add(transaction);

                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            queuedGiver.Amount--;
                            queuedGiver.ObjectState = (queuedGiver.Amount == 0)
                                ? ObjectState.Deleted
                                : ObjectState.Modified;

                            queuedReceiver.Amount--;
                            queuedReceiver.ObjectState = (queuedReceiver.Amount == 0)
                                ? ObjectState.Deleted
                                : ObjectState.Modified;

                            context.MoneyTransactions.Add(transaction);

                            queuedGiver.Account.State = AccountState.InGiveTransaction;
                            queuedGiver.Account.CurrentTransactionGroupId = transaction.WaitingGiverId;
                            queuedGiver.Account.ObjectState = ObjectState.Modified;

                            queuedReceiver.Account.State = AccountState.InReceiveTransaction;
                            queuedReceiver.Account.CurrentTransactionGroupId = transaction.WaitingReceiverId;
                            queuedReceiver.Account.ObjectState = ObjectState.Modified;

                            context.SaveChanges();

                            dbContextTransaction.Commit();
                        }
                        catch (Exception)
                        {
                            dbContextTransaction.Rollback();

                            transaction.ObjectState = ObjectState.Unchanged;

                            queuedGiver.Amount++;
                            queuedGiver.ObjectState = ObjectState.Unchanged;

                            queuedReceiver.Amount++;
                            queuedReceiver.ObjectState = ObjectState.Unchanged;

                            queuedGiver.Account.State = queuedGiverOldState;
                            queuedGiver.Account.ObjectState = ObjectState.Unchanged;

                            queuedReceiver.Account.State = queuedReceiverOldState;
                            queuedReceiver.Account.ObjectState = ObjectState.Unchanged;
                        }
                    }
                }
            }
        }

        public static void UpdateTransactions()
        {
            UpdateNotTransferedTransactions();
            UpdateNotConfirmedTransactions();
        }

        public static void UpdateNotTransferedTransactions()
        {
            using (var context = new BSE365Context())
            {
                var timeBase = DateTime.Now;
                timeBase = timeBase.AddHours(-48);
                var transactionsToUpdate = context.MoneyTransactions
                    .Include(x => x.Giver.UserInfo)
                    .Include(x => x.Receiver.UserInfo)
                    .Where(x => !x.IsEnd && x.TransferedDate < timeBase && x.State == TransactionState.Begin)
                    .ToList();
                foreach (var transaction in transactionsToUpdate)
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var giverParentAccount = context.Accounts
                                .FirstOrDefault(x => x.UserName == transaction.Giver.UserInfo.ParentId);
                            // update transaction
                            transaction.NotTransfer(giverParentAccount);


                            context.SaveChanges();

                            dbContextTransaction.Commit();
                        }
                        catch (Exception)
                        {
                            dbContextTransaction.Rollback();
                        }
                    }
                }
            }
        }

        public static void UpdateNotConfirmedTransactions()
        {
            using (var context = new BSE365Context())
            {
                var timeBase = DateTime.Now;
                timeBase = timeBase.AddHours(-48);
                var transactionsToUpdate = context.MoneyTransactions
                    .Include(x => x.Giver.UserInfo)
                    .Include(x => x.Receiver.UserInfo)
                    .Include(x => x.Giver.WaitingGivers)
                    .Where(x => !x.IsEnd && x.TransferedDate < timeBase && x.State == TransactionState.Transfered)
                    .OrderBy(x => x.LastModified)
                    .ToList();
                var giveGroupIds = transactionsToUpdate.Select(x => x.WaitingGiverId).ToArray();
                var relatedTransactions = context.MoneyTransactions
                    .Where(x => giveGroupIds.Contains(x.WaitingGiverId))
                    .ToList();
                foreach (var transaction in transactionsToUpdate)
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var otherGivingTransactionsInCurrentTransaction = relatedTransactions.Where(x =>
                                x.WaitingGiverId == transaction.WaitingGiverId && x.Id != transaction.Id)
                                .ToList();
                            // update transaction
                            transaction.NotConfirm(otherGivingTransactionsInCurrentTransaction);

                            context.SaveChanges();

                            dbContextTransaction.Commit();
                        }
                        catch (Exception)
                        {
                            dbContextTransaction.Rollback();
                        }
                    }
                }
            }
        }
    }
}