using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static double WaitingHelperTimeout = /*60*60*1000;*/ 10*60*1000; //for test
    }
}