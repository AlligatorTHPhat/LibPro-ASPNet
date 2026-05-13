/****
Tran Hoang Phat - 49.01.104.107
****/

using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryDomain.Enums
{
    public enum LoanStatus
    {
        Active = 1,

        Returned = 2,

        Overdue = 3,

        PendingFine = 4,

        Closed = 5,

        Lost = 6
    }
}
