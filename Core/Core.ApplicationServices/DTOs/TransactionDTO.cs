using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ApplicationServices.DTOs
{
    public class TransactionDTO
    {
        public string Id { get; set; }
        public string WalletId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionType TransactionType { get; set; }
        public string RelatedWalletJmbg { get; set; }
        public string ReferenceTransactionId { get; set; }
        public TransactionDTO()
        {
        }

        public TransactionDTO(Transaction transaction)
        {
            Id = transaction.Id;
            WalletId = transaction.WalletId;
            Amount = transaction.Amount;
            TransactionDate = transaction.TransactionDate;
            TransactionType = transaction.TransactionType;
            RelatedWalletJmbg = transaction.RelatedWalletJmbg;
            ReferenceTransactionId = transaction.ReferenceTransactionId;
        }
    }
}
