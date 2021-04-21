using Core.ApplicationServices.DTOs;
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

        public async Task<IEnumerable<TransactionDTO>> GetTransactionsByWalletId(string walletId)
        {
            var transactions = await UnitOfWork.TransactionRepository.GetFilteredList(w => w.WalletId == walletId);

            List<TransactionDTO> transactionDTOs = new List<TransactionDTO>();
            foreach(var transaction in transactions)
            {
                transactionDTOs.Add(new TransactionDTO(transaction));
            }

            return transactionDTOs;
        }
    }
}
