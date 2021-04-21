using Core.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Entities
{
    public class Transaction
    {
        public string Id { get; private set; }
        public string WalletId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime TransactionDate { get; private set; }
        public TransactionType TransactionType { get; private set; }
        public string RelatedWalletJmbg { get; private set; }
        public string ReferenceTransactionId { get; private set; }
        public Transaction()
        {
            Id = Guid.NewGuid().ToString();
        }

        public Transaction(string walletId,
            decimal amount,
            TransactionType transactionType) : this()
        {
            SetWalletId(walletId);
            Amount = amount;
            TransactionType = transactionType;
        }

        private void SetReferenceTransactionId(string referenceTransactionId)
        {
            if (String.IsNullOrWhiteSpace(referenceTransactionId))
            {
                throw new TransactionEntityException("ReferenceTransactionId ne sme biti null!",
                    "SetReferenceTransactionId: ReferenceTransactionId can't be null.");
            }

            ReferenceTransactionId = referenceTransactionId;
        }
        private void SetWalletId(string walletId)
        {
            if (String.IsNullOrWhiteSpace(walletId))
            {
                throw new TransactionEntityException("WalletId ne sme biti null!",
                    "SetWalletId: WalletId can't be null.");
            }

            WalletId = walletId;
        }
        private void SetRelatedJmbg(string relatedWalletJmbg)
        {
            if (String.IsNullOrWhiteSpace(relatedWalletJmbg))
            {
                throw new TransactionEntityException("WalletId ne sme biti null!",
                    "SetWalletId: WalletId can't be null.");
            }

            RelatedWalletJmbg = relatedWalletJmbg;
        }

        public void SetRelatedWalletReference(string referenceTransactionId, string relatedWalletJmbg)
        {
            SetReferenceTransactionId(referenceTransactionId);
            SetRelatedJmbg(relatedWalletJmbg);
        }
    }
}
