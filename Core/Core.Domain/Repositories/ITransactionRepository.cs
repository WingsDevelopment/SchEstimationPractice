using Common.Utils;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Repositories
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<bool> HasAnyTransferThisMonth(string walletId);

        Task<decimal> GetTransactionSumByTransactionsTypeThisMonth(string walletId, TransactionType transactionType);
    }
}
