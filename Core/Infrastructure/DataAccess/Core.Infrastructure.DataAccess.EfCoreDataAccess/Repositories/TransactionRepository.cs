using Common.EfCoreDataAccess;
using Core.Domain.Entities;
using Core.Domain.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Infrastructure.DataAccess.EfCoreDataAccess.Repositories
{
    public class TransactionRepository : EfCoreRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(CoreEfCoreDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<bool> HasAnyTransferThisMonth(string walletId)
        {
            DateTime dateTime = DateTime.Now;
            var transactions = await base.GetFilteredList(w => w.WalletId == walletId &&
                    w.TransactionType == TransactionType.TransferOut &&
                    w.TransactionDate.Month == dateTime.Month &&
                    w.TransactionDate.Year == dateTime.Year);

            if (transactions.Count == 0) return false;
            else return true;
        }

        public async Task<decimal> GetTransactionSumByTransactionsTypeThisMonth(string walletId, TransactionType transactionType)
        {
            DateTime dateTime = DateTime.Now;
            var transactions = await base.GetFilteredList(w => w.WalletId == walletId &&
                    w.TransactionType == transactionType &&
                    w.TransactionDate.Month == dateTime.Month &&
                    w.TransactionDate.Year == dateTime.Year);

            return transactions.Select(m => m.Amount).Sum();
        }
    }
}
