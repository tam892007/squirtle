using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using BSE365.Base.Infrastructures;
using BSE365.Common.Constants;
using BSE365.Model.Entities;
using BSE365.Model.Enum;
using BSE365.Repository.DataContext;

namespace BSE365.Repository.Helper
{
    public class StoreHelper
    {
        public static int MapWaitingReceiver(int waitingReceiverId)
        {
            var result = 0;
            using (var context = new BSE365Context())
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
                    .OrderBy(x => x.Created)
                    .Take(waitingReceiver.Amount)
                    .ToList();


                // update data to database
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var now = DateTime.Now;
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

                            waitingGiver.Amount--;
                            waitingGiver.ObjectState = waitingGiver.Amount == 0
                                ? ObjectState.Deleted
                                : ObjectState.Modified;

                            context.MoneyTransactions.Add(transaction);

                            // update giver's account
                            if (waitingGiver.Account.State == AccountState.WaitGive)
                            {
                                waitingGiver.Account.State = AccountState.InGiveTransaction;
                            }
                            waitingGiver.Account.CurrentTransactionGroupId = waitingGiver.Id;
                            waitingGiver.Account.ObjectState = ObjectState.Modified;
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
                            waitingReceiver.Account.CurrentTransactionGroupId = waitingReceiver.Id;
                            waitingReceiver.Account.ObjectState = ObjectState.Modified;
                        }

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
                    using (var dbContextTransaction = context.Database.BeginTransaction())
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

                            context.MoneyTransactions.Add(transaction);

                            // update giver's account
                            if (queuedGiveRaw.Account.State == AccountState.WaitGive)
                            {
                                queuedGiveRaw.Account.State = AccountState.InGiveTransaction;
                            }
                            queuedGiveRaw.Account.CurrentTransactionGroupId = transaction.WaitingGiverId;
                            queuedGiveRaw.Account.ObjectState = ObjectState.Modified;

                            if (queuedReceiveRaw.Type != WaitingType.Bonus)
                            {
                                // update receiver's account
                                if (queuedReceiveRaw.Account.State == AccountState.WaitReceive)
                                {
                                    queuedReceiveRaw.Account.State = AccountState.InReceiveTransaction;
                                }
                                queuedReceiveRaw.Account.CurrentTransactionGroupId = transaction.WaitingReceiverId;
                                queuedReceiveRaw.Account.ObjectState = ObjectState.Modified;
                            }

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
            var existReceiverIdsInGiverTransaction = context.MoneyTransactions.Where(
                x => x.GiverId == giveRequest.AccountId &&
                     x.WaitingGiverId == giveRequest.Id)
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
                    var transactionsToUpdate = context.MoneyTransactions
                        .Include(x => x.Giver.UserInfo)
                        .Include(x => x.Receiver.UserInfo)
                        .Where(x => !x.IsEnd && x.Created < timeBase && x.State == TransactionState.Begin)
                        .ToList();
                    foreach (var transaction in transactionsToUpdate)
                    {
                        using (var dbContextTransaction = context.Database.BeginTransaction())
                        {
                            try
                            {
                                Account giverParentAccount = null;
                                var giverParentId = transaction.Giver.UserInfo.ParentId;
                                if (!string.IsNullOrEmpty(giverParentId))
                                {
                                    var giverParentAuthAccount = authContext.Users
                                        .FirstOrDefault(x => x.Id == giverParentId);
                                    if (giverParentAuthAccount != null)
                                    {
                                        giverParentAccount = context.Accounts
                                            .FirstOrDefault(x => x.UserName == giverParentAuthAccount.UserName);
                                    }
                                }

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
        }

        public static void UpdateNotConfirmedTransactions()
        {
            using (var context = new BSE365Context())
            {
                using (var authContext = new BSE365AuthContext())
                {
                    var timeBase = DateTime.Now;
                    timeBase = timeBase.AddHours(-TransactionConfig.TimeForEachStepInHours);
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

                                var giverParentInfoIds = new List<int>();
                                var giverTreePath = transaction.Giver.UserInfo.TreePath;
                                if (!string.IsNullOrEmpty(giverTreePath))
                                {
                                    var giverParentIds = giverTreePath.Split(
                                        new string[] {BSE365.Common.Constants.SystemAdmin.TreePathSplitter},
                                        StringSplitOptions.RemoveEmptyEntries);
                                    giverParentInfoIds = authContext.Users
                                        .Where(x => giverParentIds.Contains(x.Id))
                                        .Select(x => x.UserInfo.Id).ToList();
                                }
                                var giverParentInfos = context.UserInfos
                                    .Where(x => giverParentInfoIds.Contains(x.Id)).ToList();

                                // update transaction
                                transaction.NotConfirm(otherGivingTransactionsInCurrentTransaction, giverParentInfos);

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
}