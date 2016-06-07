namespace BSE365.Common.Constants
{
    public static class TransactionConfig
    {
        /// <summary>
        /// in millisecond
        /// </summary>
        public static double TransactionCheckerTimeout = /*60*60*1000;*/ 1*60*1000; //for test

        /// <summary>
        /// in millisecond
        /// </summary>
        public static double WaitingHelperTimeout = /*60*60*1000;*/ 1*60*1000; //for test

        /// <summary>
        /// Time for each step of Transaction in hours
        /// </summary>
        public static int TimeForEachStepInHours = 32;

        public static int GiveAmountDefault = 2;
        public static int GiveAmountAbadon = 1;

        public static int ReceiveAmountDefault = 3;
    }
}