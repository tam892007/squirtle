using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE365.Model.Enum
{
    public enum UserState
    {
        /// <summary>
        /// Default
        /// </summary>
        Default = 0,

        NotGive = 21,
        NotConfirm = 22,
        ReportedNotTransfer = 23,

        AbadonedOne = 31,
    }
}