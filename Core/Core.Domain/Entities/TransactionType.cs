using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Entities
{
    public enum TransactionType
    {
        Withdraw = 1,
        Deposit = 2,
        TransferOut = 11,
        TransferIn = 12,
    }
}
