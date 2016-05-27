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

        NotTransfer = 11,
        NotConfirm = 12,
        ReportedNotTransfer = 13,
    }
}