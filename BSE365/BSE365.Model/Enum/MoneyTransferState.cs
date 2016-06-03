using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE365.Model.Enum
{
    public enum MoneyTransferState
    {
        Begin = 0,
        Transfered = 1,
        Confirmed = 2,

        NotTransfer = 21,
        NotConfirm = 22,
        ReportedNotTransfer = 23,
    }
}