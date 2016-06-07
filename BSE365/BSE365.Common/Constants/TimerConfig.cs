namespace BSE365.Common.Constants
{
    public static class TimerConfig
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
        /// Hours
        /// </summary>
        public static int TimeForEachStepInHours = 32;

        /// <summary>
        /// Milliseconds
        /// </summary>
        public static int TimeForEachStepInSeconds
        {
            get { return TimeForEachStepInHours*60*60; }
        }
    }
}