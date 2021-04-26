using Core.ApplicationServices.DTOs;
using Core.Domain.Entities;
using System;

namespace Core.ApplicationServices.Factories
{
    public class TransferFactory
    {
        private readonly decimal PercentageCommissionStartingAmount;
        private readonly decimal FixedCommission;
        private readonly decimal PercentageCommission;

        public TransferFactory(string percentageCommissionStartingAmount,
            string fixedCommission,
            string percentageCommission)
        {
            if (!decimal.TryParse(percentageCommissionStartingAmount, out PercentageCommissionStartingAmount)) 
                throw new ArgumentException("Invalid PercentageCommissionStartingAmount string");

            if (!decimal.TryParse(fixedCommission, out FixedCommission))
                throw new ArgumentException("Invalid FixedCommission string");

            if (!decimal.TryParse(percentageCommission, out PercentageCommission))
                throw new ArgumentException("Invalid PercentageCommission string");
        }

        /// <summary>
        /// Makes the transfer, applies comission and updates wallets
        /// </summary>
        /// <param name="walletFrom"></param>
        /// <param name="walletTo"></param>
        /// <param name="amount"></param>
        /// <returns>Transfer with two transactions</returns>
        public TransferDTO CreateTransferAndUpdateWallets(Wallet walletFrom, Wallet walletTo, decimal amount)
        {
            decimal transferInAmount = amount;
            decimal transferOutAmount = amount;

            //discount
            transferOutAmount = GetAmountAfterComission(transferOutAmount);

            walletFrom.Withdraw(transferOutAmount);
            walletTo.Deposit(transferInAmount);

            Transaction transferOut = new Transaction(walletFrom.Id, transferOutAmount, TransactionType.TransferOut);
            Transaction transferIn = new Transaction(walletTo.Id, transferInAmount, TransactionType.TransferIn);

            transferOut.SetTransferReference(transferIn.Id, walletTo.JMBG);
            transferIn.SetTransferReference(transferOut.Id, walletFrom.JMBG);

            return new TransferDTO(transferOut, transferIn);
        }

        /// <summary>
        /// Makes the transfer, and updates wallets
        /// </summary>
        /// <param name="walletFrom"></param>
        /// <param name="walletTo"></param>
        /// <param name="amount"></param>
        /// <returns>Transfer with two transactions</returns>
        public TransferDTO CreateTransferAndUpdateWalletsWithoutComission(Wallet walletFrom, Wallet walletTo, decimal amount)
        {
            walletFrom.Withdraw(amount);
            walletTo.Deposit(amount);

            Transaction transferOut = new Transaction(walletFrom.Id, amount, TransactionType.TransferOut);
            Transaction transferIn = new Transaction(walletTo.Id, amount, TransactionType.TransferIn);

            transferOut.SetTransferReference(transferIn.Id, walletTo.JMBG);
            transferIn.SetTransferReference(transferOut.Id, walletFrom.JMBG);

            return new TransferDTO(transferOut, transferIn);
        }

        private decimal GetAmountAfterComission(decimal amount)
        {
            decimal comission;
            if (amount < PercentageCommissionStartingAmount)
            {
                comission = FixedCommission;
            }
            else
            {
                comission = amount * PercentageCommission;
            }

            return amount + comission;
        }
    }
}
