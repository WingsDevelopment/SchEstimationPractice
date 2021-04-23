using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ApplicationServices.DTOs
{
    public class TransferDTO
    {
        public Transaction TransactionOut { get; set; }
        public Transaction TransactionIn { get; set; }

        public TransferDTO(Transaction transactionFrom, Transaction transactionTo)
        {
            TransactionOut = transactionFrom;
            TransactionIn = transactionTo;
        }
    }
}
