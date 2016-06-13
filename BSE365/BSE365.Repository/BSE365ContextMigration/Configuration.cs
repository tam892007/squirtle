using BSE365.Base.Infrastructures;
using BSE365.Common.Constants;
using BSE365.Common.Helper;
using BSE365.Model.Entities;
using BSE365.Model.Enum;

namespace BSE365.Repository.BSE365ContextMigration
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<BSE365.Repository.DataContext.BSE365Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"BSE365ContextMigration";
            ContextKey = "BSE365.Repository.DataContext.BSE365Context";
        }

        protected override void Seed(BSE365.Repository.DataContext.BSE365Context context)
        {
            return;
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            //InitData(context);
            //var accountCount = context.Accounts.Count();
            //if (accountCount == 1)
            //{
            //    CreateAccount(context);
            //    QueueWaitingList(context);
            //}
        }

        public static void CreateAccount(BSE365.Repository.DataContext.BSE365Context context)
        {
            var userinfos = context.UserInfos.ToList();
            for (int i = 0; i < TestData.BankNumbers.Length; i++)
            {
                var banknumber = TestData.BankNumbers[i];
                var info = userinfos.First(x => x.BankNumber == banknumber);
                var userid = Utilities.StandardizeUserId(info.Id);
                foreach (var suffix in TestData.AccountSuffixes)
                {
                    var username = userid + suffix;
                    var account = new Account
                    {
                        UserName = username,
                        UserInfoId = info.Id,
                        ObjectState = ObjectState.Added,
                    };
                    context.Accounts.Add(account);
                }
            }
            context.SaveChanges();
        }

        public static void QueueWaitingList(BSE365.Repository.DataContext.BSE365Context context)
        {
            var accounts = context.Accounts.Where(x => x.UserInfoId != 1).ToList();
            for (int i = 0; i < accounts.Count; i++)
            {
                var account = accounts[i];
                if (i%3 == 0)
                {
                    account.State = AccountState.WaitReceive;
                    account.LastCycleDate = DateTime.Now.Date;
                    account.ObjectState = ObjectState.Modified;
                    account.WaitingReceivers.Add(new WaitingReceiver
                    {
                        Created = DateTime.Now,
                        ObjectState = ObjectState.Added,
                    });
                }
                else
                {
                    account.State = AccountState.WaitGive;
                    account.LastCycleDate = DateTime.Now.Date;
                    account.ObjectState = ObjectState.Modified;
                    account.WaitingGivers.Add(new WaitingGiver
                    {
                        Created = DateTime.Now,
                        ObjectState = ObjectState.Added,
                    });
                }
            }
            context.SaveChanges();
        }

        public static void ClearWaitingTransactionData(BSE365.Repository.DataContext.BSE365Context context)
        {
            var accounts = context.Accounts
                .Include(x => x.UserInfo)
                .Where(x => x.UserInfoId != 1)
                .ToList();
            foreach (var account in accounts)
            {
                account.State = AccountState.Default;
                account.LastCycleDate = DateTime.Now.AddDays(-8);
                account.CurrentTransactionGroupId = null;
                account.RelatedTransaction = string.Empty;
                account.ObjectState = ObjectState.Modified;

                var info = account.UserInfo;
                info.State = UserState.Default;
                info.LastGiveDate = DateTime.Now.AddDays(-1);
                info.GiveOver = -1;
                info.IsAllowAbandonOne = false;
                info.RelatedAccount = null;
                info.ObjectState = ObjectState.Modified;
            }
            var transactions = context.MoneyTransactions.ToList();
            foreach (var item in transactions)
            {
                item.ObjectState = ObjectState.Deleted;
            }
            var groups = context.MoneyTransferGroups.ToList();
            foreach (var item in groups)
            {
                item.ObjectState = ObjectState.Deleted;
            }
            var wgivers = context.WaitingGivers.ToList();
            foreach (var item in wgivers)
            {
                item.ObjectState = ObjectState.Deleted;
            }
            var wreceivers = context.WaitingReceivers.ToList();
            foreach (var item in wreceivers)
            {
                item.ObjectState = ObjectState.Deleted;
            }
            context.SaveChanges();
        }
    }
}