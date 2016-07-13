namespace BSE365.Common.Constants
{
    public static class TransactionConfig
    {
        /// <summary>
        /// in millisecond
        /// </summary>
        public static double TransactionCheckerTimeout = /*60*60*1000;*/ 30*60*1000; //for test

        /// <summary>
        /// in millisecond
        /// </summary>
        public static double MappingHelperTimeout = /*60*60*1000;*/ 60*60*1000; //for test

        /// <summary>
        /// in millisecond
        /// </summary>
        public static double WaitingHelperTimeout = /*60*60*1000;*/ 30*60*1000; //for test

        /// <summary>
        /// Time for each step of Transaction in hours
        /// </summary>
        public static int TimeForEachStepInHours = 48;

        public static int GiveAmountDefault = 2;
        public static int GiveAmountAbandon = 1;

        public static int ReceiveAmountDefault = 3;
        public static int ReceiveAmountBonus = 1;

        public static int GiveOverToQueueReceive = 2;

        public static int MoneyPerTransaction = 1500000;
        public static string MoneyCurrency = "VND";

        public static int BonusPointToParent = 6;
        public static int BonusPointToTreePath = 1;
        public static int BonusMaxLevel = 5;

        public static int BonusPointToExchange = 100;
    }
}