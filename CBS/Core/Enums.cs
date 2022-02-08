using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace accAfpslaiEmvObjct.CBS.Core
{
    public enum TransactionStatus
    {
        New = 0,
        OverrideApproved = 1,
        Reversal = 2
    }

    public enum ProcessingMode
    {
        Online = 0,
        Batch = 1,
        EOD = 2
    }
}
