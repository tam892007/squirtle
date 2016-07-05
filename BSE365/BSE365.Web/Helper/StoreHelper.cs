using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using BSE365.Base.Infrastructures;
using BSE365.Common.Constants;
using BSE365.Model.Entities;
using BSE365.Model.Enum;
using BSE365.Repository.DataContext;
using Hangfire;

namespace BSE365.Helper
{
    public class StoreHelper
    {
        public static int MapWaitingReceiver(int waitingReceiverId)
        {
            int result;
            var giverIds = new List<string>();
            var giverEmails = new List<string>();
            var receiverId = string.Empty;
            var receiverEmail = string.Empty;
            using (var context = new BSE365Context())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction(IsolationLevel.RepeatableRead))
                {
                    var waitingReceiver = context.WaitingReceivers
                    .Include(x => x.Account.UserInfo)
                    .First(x => x.Id == waitingReceiverId);

                var existGiverIdsInGiverTransaction = context.MoneyTransactions
                    .Where(x => x.WaitingReceiverId == waitingReceiverId)
                    .Select(x => x.GiverId).ToList();

                var waitingGivers = context.WaitingGivers
                    .Include(x => x.Account.UserInfo)
                    .Where(x => !existGiverIdsInGiverTransaction.Contains(x.AccountId))
                    .OrderByDescending(x => x.Priority)
                    .ThenBy(x => x.Created)
                    .Take(waitingReceiver.Amount)
                    .ToList();

                // fill data to notify
                giverIds = waitingGivers.Select(x => x.AccountId).ToList();
                giverEmails = waitingGivers.Select(x => x.Account.UserInfo.Email).ToList();
                receiverId = waitingReceiver.AccountId;
                receiverEmail = waitingReceiver.Account.UserInfo.Email;

                // update data to database
                    try
                    {
                        var now = DateTime.Now;
                        var transactionsToAdd = new List<MoneyTransaction>();
                        foreach (var waitingGiver in waitingGivers)
                        {
                            var transaction = new MoneyTransaction
                            {
                                GiverId = waitingGiver.AccountId,
                                ReceiverId = waitingReceiver.AccountId,
                                Created = now,
                                LastModified = now,
                                WaitingGiverId = waitingGiver.Id,
                                WaitingReceiverId = waitingReceiver.Id,
                                ObjectState = ObjectState.Added
                            };

                            if (waitingReceiver.Type == WaitingType.Bonus)
                            {
                                transaction.Type = TransactionType.Bonus;
                            }

                            // update waiting raw
                            waitingGiver.Amount--;
                            waitingGiver.ObjectState = waitingGiver.Amount == 0
                                ? ObjectState.Deleted
                                : ObjectState.Modified;

                            // update giver's account
                            if (waitingGiver.Account.State == AccountState.WaitGive)
                            {
                                waitingGiver.Account.State = AccountState.InGiveTransaction;
                            }
                            // update current transaction group id if needed
                            if (waitingGiver.Account.CurrentTransactionGroupId == null)
                            {
                                waitingGiver.Account.CurrentTransactionGroupId = waitingGiver.Id;
                            }
                            else
                            {
                                transaction.WaitingGiverId = waitingGiver.Account.CurrentTransactionGroupId.Value;
                            }
                            waitingGiver.Account.ObjectState = ObjectState.Modified;

                            transactionsToAdd.Add(transaction);
                        }

                        waitingReceiver.Amount -= waitingGivers.Count;
                        result = waitingReceiver.Amount;
                        waitingReceiver.ObjectState = waitingReceiver.Amount == 0
                            ? ObjectState.Deleted
                            : ObjectState.Modified;

                        if (waitingReceiver.Type != WaitingType.Bonus)
                        {
                            // update receiver's account
                            if (waitingReceiver.Account.State == AccountState.WaitReceive)
                            {
                                waitingReceiver.Account.State = AccountState.InReceiveTransaction;
                            }
                            // update current transaction group id if needed
                            if (waitingReceiver.Account.CurrentTransactionGroupId == null)
                            {
                                waitingReceiver.Account.CurrentTransactionGroupId = waitingReceiver.Id;
                            }
                            else
                            {
                                transactionsToAdd.ForEach(
                                    x =>
                                    {
                                        x.WaitingReceiverId = waitingReceiver.Account.CurrentTransactionGroupId.Value;
                                    });
                            }
                            waitingReceiver.Account.ObjectState = ObjectState.Modified;
                        }

                        // add transactions
                        transactionsToAdd.ForEach(x => { context.MoneyTransactions.Add(x); });

                        context.SaveChanges();

                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                        throw;
                    }
                }
            }
            BackgroundJob.Enqueue(() => NotificationHelper.NotifyTransactionMapped(
                giverIds, receiverId));
            BackgroundJob.Enqueue(() => EmailHelper.NotifyTransactionMapped(
                giverIds, giverEmails, receiverId, receiverEmail,
                string.Empty));
            return result;
        }


        public static void MapWaitingGiversAndWaitingReceivers()
        {
            var now = DateTime.Now;
            using (var context = new BSE365Context())
            {
                // get queued givers and queued receivers
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
                // transform queued data single queue (split amount)
                var transformedWaitingGivers = new List<WaitingBase>();
                var transformedWaitingReceivers = new List<WaitingBase>();
                foreach (var queued in queuedWaitingGivers)
                {
                    for (var i = 0; i < queued.Amount; i++)
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
                    for (var i = 0; i < queued.Amount; i++)
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

                while (transformedWaitingGivers.Count > 0 && transformedWaitingReceivers.Count > 0)
                {
                    var coupleMapped = GetMappedCouple(context, transformedWaitingGivers, transformedWaitingReceivers);
                    if (!coupleMapped.Item1)
                    {
                        break;
                    }

                    var giveRequest = coupleMapped.Item2;
                    var receiveRequest = coupleMapped.Item3;


                    // select raw waiting data
                    var queuedGiveRaw = queuedWaitingGivers.First(x => x.Id == giveRequest.Id);
                    var queuedReceiveRaw = queuedWaitingReceivers.First(x => x.Id == receiveRequest.Id);

                    // save account state for rollback
                    var giverOldState = queuedGiveRaw.Account.State;
                    var receiverOldState = queuedReceiveRaw.Account.State;


                    var transaction = new MoneyTransaction
                    {
                        GiverId = giveRequest.AccountId,
                        ReceiverId = receiveRequest.AccountId,
                        Created = now,
                        LastModified = now,
                        WaitingGiverId = queuedGiveRaw.Id,
                        WaitingReceiverId = queuedReceiveRaw.Id,
                        ObjectState = ObjectState.Added
                    };

                    if (queuedReceiveRaw.Type == WaitingType.Bonus)
                    {
                        transaction.Type = TransactionType.Bonus;
                    }


                    // update data to database
                    using (var dbContextTransaction = context.Database.BeginTransaction(IsolationLevel.RepeatableRead))
                    {
                        try
                        {
                            // update waiting raw
                            queuedGiveRaw.Amount--;
                            queuedGiveRaw.ObjectState = queuedGiveRaw.Amount == 0
                                ? ObjectState.Deleted
                                : ObjectState.Modified;

                            queuedReceiveRaw.Amount--;
                            queuedReceiveRaw.ObjectState = queuedReceiveRaw.Amount == 0
                                ? ObjectState.Deleted
                                : ObjectState.Modified;

                            // update giver's account
                            if (queuedGiveRaw.Account.State == AccountState.WaitGive)
                            {
                                queuedGiveRaw.Account.State = AccountState.InGiveTransaction;
                            }
                            // update current transaction group id if needed
                            if (queuedGiveRaw.Account.CurrentTransactionGroupId == null)
                            {
                                queuedGiveRaw.Account.CurrentTransactionGroupId = queuedGiveRaw.Id;
                            }
                            else
                            {
                                transaction.WaitingGiverId =
                                    queuedGiveRaw.Account.CurrentTransactionGroupId.Value;
                            }
                            queuedGiveRaw.Account.ObjectState = ObjectState.Modified;

                            if (queuedReceiveRaw.Type != WaitingType.Bonus)
                            {
                                // update receiver's account
                                if (queuedReceiveRaw.Account.State == AccountState.WaitReceive)
                                {
                                    queuedReceiveRaw.Account.State = AccountState.InReceiveTransaction;
                                }
                                // update current transaction group id if needed
                                if (queuedReceiveRaw.Account.CurrentTransactionGroupId == null)
                                {
                                    queuedReceiveRaw.Account.CurrentTransactionGroupId = queuedReceiveRaw.Id;
                                }
                                else
                                {
                                    transaction.WaitingReceiverId =
                                        queuedReceiveRaw.Account.CurrentTransactionGroupId.Value;
                                }
                                queuedReceiveRaw.Account.ObjectState = ObjectState.Modified;
                            }

                            // add transaction
                            context.MoneyTransactions.Add(transaction);

                            context.SaveChanges();

                            dbContextTransaction.Commit();
                        }
                        catch (Exception)
                        {
                            dbContextTransaction.Rollback();

                            transaction.ObjectState = ObjectState.Unchanged;

                            queuedGiveRaw.Amount++;
                            queuedGiveRaw.ObjectState = ObjectState.Unchanged;

                            queuedReceiveRaw.Amount++;
                            queuedReceiveRaw.ObjectState = ObjectState.Unchanged;

                            queuedGiveRaw.Account.State = giverOldState;
                            queuedGiveRaw.Account.ObjectState = ObjectState.Unchanged;

                            queuedReceiveRaw.Account.State = receiverOldState;
                            queuedReceiveRaw.Account.ObjectState = ObjectState.Unchanged;
                        }
                    }
                }
            }
        }


        private static Tuple<bool, WaitingBase, WaitingBase> GetMappedCouple(
            BSE365Context context,
            List<WaitingBase> transformedWaitingGivers,
            List<WaitingBase> transformedWaitingReceivers)
        {
            var isSuccessed = true;

            var countTried = 0;

            // select single give request
            var giveRequest = transformedWaitingGivers[0];
            transformedWaitingGivers.RemoveAt(0);

            Check_Duplicate_Point:
            countTried++;
            // check duplicate
            var existReceiverIdsInGiverTransaction = context.MoneyTransactions
                .Where(x => x.GiverId == giveRequest.AccountId && x.WaitingGiverId == giveRequest.Id)
                .Select(x => x.ReceiverId);

            // select single receive request
            var receiveRequest = transformedWaitingReceivers
                .FirstOrDefault(x => !existReceiverIdsInGiverTransaction.Contains(x.AccountId));

            if (receiveRequest != null)
            {
                transformedWaitingReceivers.Remove(receiveRequest);
            }
            else
            {
                if (countTried == transformedWaitingGivers.Count)
                {
                    isSuccessed = false;
                }
                else
                {
                    transformedWaitingGivers.Add(giveRequest);

                    giveRequest = transformedWaitingGivers[0];
                    transformedWaitingGivers.RemoveAt(0);

                    goto Check_Duplicate_Point;
                }
            }

            var result = new Tuple<bool, WaitingBase, WaitingBase>(isSuccessed, giveRequest, receiveRequest);
            return result;
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
                using (var authContext = new BSE365AuthContext())
                {
                    var timeBase = DateTime.Now;
                    timeBase = timeBase.AddHours(-TransactionConfig.TimeForEachStepInHours);
                    using (var dbContextTransaction = context.Database.BeginTransaction(IsolationLevel.RepeatableRead))
                    {
                        var transactionsToUpdate = context.MoneyTransactions
                        .Include(x => x.Giver.UserInfo)
                        .Include(x => x.Receiver.UserInfo)
                        .Where(x => !x.IsEnd && x.Created < timeBase && x.State == TransactionState.Begin)
                        .ToList();
                    foreach (var transaction in transactionsToUpdate)
                    {
                            try
                            {
                                Account giverParentAccount = null;
                                var giverParentId = transaction.Giver.UserInfo.ParentId;
                                string giverParentEmail = null;
                                string giverParentAccountId = null;
                                if (!string.IsNullOrEmpty(giverParentId))
                                {
                                    var giverParentAuthAccount = authContext.Users
                                        .FirstOrDefault(x => x.Id == giverParentId);
                                    if (giverParentAuthAccount != null)
                                    {
                                        giverParentAccount = context.Accounts
                                            .Include(x => x.UserInfo)
                                            .FirstOrDefault(x => x.UserName == giverParentAuthAccount.UserName);
                                        if (giverParentAccount != null)
                                        {
                                            giverParentEmail = giverParentAccount.UserInfo.Email;
                                            giverParentAccountId = giverParentAccount.UserName;
                                        }
                                    }
                                }

                                // update transaction
                                transaction.NotTransfer(giverParentAccount);

                                BackgroundJob.Enqueue(() => NotificationHelper.NotifyTransactionNotTransfered(
                                    transaction.Id,
                                    transaction.GiverId, transaction.ReceiverId,
                                    giverParentAccountId));
                                BackgroundJob.Enqueue(() => EmailHelper.NotifyTransactionNotTransfered(transaction.Id,
                                    transaction.GiverId, transaction.Giver.UserInfo.Email,
                                    transaction.ReceiverId, transaction.Receiver.UserInfo.Email,
                                    giverParentAccountId, giverParentEmail));

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

        public static void UpdateNotConfirmedTransactions()
        {
            using (var context = new BSE365Context())
            {
                using (var authContext = new BSE365AuthContext())
                {
                    var timeBase = DateTime.Now;
                    timeBase = timeBase.AddHours(-TransactionConfig.TimeForEachStepInHours);
                    using (var dbContextTransaction = context.Database.BeginTransaction(IsolationLevel.RepeatableRead))
                    {
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
                            try
                            {
                                var otherGivingTransactionsInCurrentTransaction = relatedTransactions.Where(x =>
                                    x.WaitingGiverId == transaction.WaitingGiverId && x.Id != transaction.Id)
                                    .ToList();

                                var giverParentInfoIds = new List<int>();
                                var giverTreePath = transaction.Giver.UserInfo.TreePath;
                                if (!string.IsNullOrEmpty(giverTreePath))
                                {
                                    var giverParentIds = giverTreePath.Split(
                                        new[] {SystemAdmin.TreePathSplitter},
                                        StringSplitOptions.RemoveEmptyEntries);
                                    giverParentInfoIds = authContext.Users
                                        .Where(x => giverParentIds.Contains(x.Id))
                                        .Select(x => x.UserInfo.Id).ToList();
                                }
                                var giverParentInfos = context.UserInfos
                                    .Where(x => giverParentInfoIds.Contains(x.Id)).ToList();

                                // update transaction
                                transaction.NotConfirm(otherGivingTransactionsInCurrentTransaction, giverParentInfos);

                                BackgroundJob.Enqueue(() => NotificationHelper.NotifyTransactionNotConfirmed(
                                    transaction.Id,
                                    transaction.GiverId, transaction.ReceiverId));
                                BackgroundJob.Enqueue(() => EmailHelper.NotifyTransactionNotConfirmed(transaction.Id,
                                    transaction.GiverId, transaction.Giver.UserInfo.Email,
                                    transaction.ReceiverId, transaction.Receiver.UserInfo.Email));

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


        public static void AutoQueueReceive()
        {
            using (var context = new BSE365Context())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction(IsolationLevel.RepeatableRead))
                {
                    var accountCanQueueReceives = context.Accounts
                    .Include(x => x.UserInfo)
                    .Where(x =>
                        x.State == AccountState.Gave &&
                        x.UserInfo.State == UserState.Default &&
                        x.UserInfo.GiveOver >= TransactionConfig.GiveOverToQueueReceive).ToList();

                // select one account for each user
                var accountToQueues = accountCanQueueReceives.GroupBy(x => x.UserInfoId).Select(x => x.First());
                foreach (var account in accountToQueues)
                {
                        WaitingReceiver waitingqueue = null;
                        try
                        {
                            waitingqueue = account.QueueReceive();

                            context.SaveChanges();

                            dbContextTransaction.Commit();
                        }
                        catch (Exception)
                        {
                            dbContextTransaction.Rollback();

                            account.ObjectState = ObjectState.Unchanged;

                            account.UserInfo.ObjectState = ObjectState.Unchanged;

                            if (waitingqueue != null)
                                waitingqueue.ObjectState = ObjectState.Unchanged;
                        }
                    }
                }
            }
        }
    }
}