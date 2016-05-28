using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE365.Model.Enum
{
    public enum AccountState
    {
        /// <summary>
        /// Must give 
        /// </summary>
        Default = 0,
        /// <summary>
        /// Queued in WaitingGive
        /// </summary>
        WaitGive = 1,
        /// <summary>
        /// Gave
        /// Can queue receive
        /// </summary>
        Gave = 2,
        /// <summary>
        /// Queued in WaitingReceive
        /// </summary>
        WaitReceive = 3,

        InTransaction = 10,
        NotGive = 11,
        NotConfirm = 12,
        ReportedNotTransfer = 13,
    }
}