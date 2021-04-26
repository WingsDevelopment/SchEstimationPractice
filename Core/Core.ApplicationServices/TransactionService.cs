using Core.ApplicationServices.DTOs;
using Core.Domain.Entities;
using Core.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.ApplicationServices
{
    public class TransactionService
    {
        private readonly ICoreUnitOfWork UnitOfWork;

        public TransactionService(
                   ICoreUnitOfWork unitOfWork
               )
        {
            UnitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TransactionDTO>> GetTransactionsByWalletIdAndDate(string walletId, DateTime? date = null)
        {
            var transactions = await UnitOfWork.TransactionRepository
                .GetFilteredList(w => w.WalletId == walletId &&
                    (date == null | w.TransactionDate.Date == date.Value.Date));

            List<TransactionDTO> transactionDTOs = new List<TransactionDTO>();
            foreach(var transaction in transactions)
            {
                transactionDTOs.Add(new TransactionDTO(transaction));
            }

            return transactionDTOs;
        }
    }
}
