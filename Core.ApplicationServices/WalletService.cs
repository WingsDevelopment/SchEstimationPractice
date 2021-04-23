using Core.ApplicationServices.DTOs;
using Core.ApplicationServices.Factories;
using Core.ApplicationServices.Exceptions;
using Core.Domain.Entities;
using Core.Domain.Repositories;
using Core.Domain.Services.External;
using Core.Domain.Services.Internal.Interfaces;
using System;
using System.Threading.Tasks;

namespace Core.ApplicationServices
{
    public class WalletService
    {
        private readonly IPassService PassService;
        private readonly ICoreUnitOfWork UnitOfWork;
        private readonly IBankService BankService;
        private readonly int NumberOfFirstDaysWithoutComission;
        private readonly decimal MaxWithdraw;
        private readonly decimal MaxDeposit;
        private readonly TransferFactory TransferFactory;

        public WalletService(
                   ICoreUnitOfWork unitOfWork,
                   IBankService rakicRaiffeisenBrosBankService,
                   IPassService passService,
                   string numberOfFirstDaysWithoutComission,
                   TransferFactory transferFactory,
                   string maxWithdraw,
                   string maxDeposit
               )
        {
            UnitOfWork = unitOfWork;
            BankService = rakicRaiffeisenBrosBankService;
            PassService = passService;
            TransferFactory = transferFactory;

            if (!int.TryParse(numberOfFirstDaysWithoutComission, out NumberOfFirstDaysWithoutComission))
                throw new ArgumentException("Invalid NumberOfFirstDaysWithoutComission string");

            if (!decimal.TryParse(maxWithdraw, out MaxWithdraw))
                throw new ArgumentException("Invalid MaxDeposit string");

            if (!decimal.TryParse(maxDeposit, out MaxDeposit))
                throw new ArgumentException("Invalid MaxDeposit string");
        }

        /// <summary>
        /// Metoda koja pravi wallet
        /// </summary>
        /// <param name="walletDTO"></param>
        /// <returns>Id</returns>
        public async Task<WalletDTO> CreateWallet(WalletDTO walletDTO)
        {
            var result = await UnitOfWork.WalletRepository.GetFirstOrDefaultWithIncludes(m => m.JMBG == walletDTO.JMBG);
            if (result != null)
            {
                throw new WalletServiceException("Novcanik sa ovim JMBG-om vec postoji!", "CreateWallet: Duplicate JMBG");
            }

            bool isValid = await BankService.CheckStatus(walletDTO.JMBG, walletDTO.PIN);
            if (!isValid) new WalletServiceException("PIN nije validan!", "CreateWallet: Invalid PIN");

            string pass = PassService.GeneratePASS();

            Wallet newWallet = new Wallet(walletDTO.FirstName,
                walletDTO.LastName,
                walletDTO.JMBG,
                walletDTO.Bank,
                walletDTO.PIN,
                walletDTO.BankAccount,
                pass);

            await UnitOfWork.WalletRepository.Insert(newWallet);
            await UnitOfWork.SaveChangesAsync();

            return new WalletDTO(newWallet);
        }

        /// <summary>
        /// Withdraw from bank, deposit to wallet
        /// </summary>
        /// <returns></returns>
        public async Task<WalletDTO> Deposit(string jmbg,
            string pass,
            decimal amount)
        {
            try
            {
                await UnitOfWork.BeginTransactionAsync();

                var wallet = await UnitOfWork.WalletRepository.GetFirstOrDefaultWithIncludes(w => w.JMBG == jmbg);
                ValidateWallet(wallet, jmbg, pass);
                if (wallet.IsBlocked) throw new WalletServiceException("Wallet je blokiran!", "Deposit: Wallet is blocked!");

                decimal thisMonthDepositSum = await UnitOfWork.TransactionRepository
                    .GetTransactionSumByTransactionsTypeThisMonth(wallet.Id, TransactionType.Deposit);
                if (thisMonthDepositSum + amount > MaxDeposit)
                {
                    throw new WalletServiceException("Vi ste terorista!", "Deposit: Max amount of deposit exceeded this month");
                }

                await BankService.Withdraw(wallet.JMBG, wallet.PIN, amount);

                Transaction transaction = new Transaction(wallet.Id, amount, TransactionType.Deposit);
                wallet.Deposit(transaction.Amount);

                await UnitOfWork.WalletRepository.Update(wallet);
                await UnitOfWork.TransactionRepository.Insert(transaction);
                await UnitOfWork.SaveChangesAsync();
                await UnitOfWork.CommitTransactionAsync();

                return new WalletDTO(wallet);
            }
            catch (Exception e)
            {
                await UnitOfWork.RollbackTransactionAsync();

                throw e;
            }
        }
        /// <summary>
        /// Withdraw from wallet, deposit to bank
        /// </summary>
        /// <returns></returns>
        public async Task<WalletDTO> Withdraw(string jmbg,
            string pass,
            decimal amount)
        {
            try
            {
                await UnitOfWork.BeginTransactionAsync();

                var wallet = await UnitOfWork.WalletRepository.GetFirstOrDefaultWithIncludes(w => w.JMBG == jmbg);
                ValidateWallet(wallet, jmbg, pass);
                if (wallet.IsBlocked) throw new WalletServiceException("Wallet je blokiran!", "Withdraw: Wallet is blocked!");

                decimal thisMonthWithdrawSum = await UnitOfWork.TransactionRepository
                    .GetTransactionSumByTransactionsTypeThisMonth(wallet.Id, TransactionType.Withdraw);
                if (thisMonthWithdrawSum + amount > MaxWithdraw)
                {
                    throw new WalletServiceException("Vi ste terorista!", "Withdraw: Max amount of withdraw exceeded this month");
                }

                await BankService.Deposit(wallet.JMBG, wallet.PIN, amount);

                Transaction transaction = new Transaction(wallet.Id, amount, TransactionType.Withdraw);
                wallet.Withdraw(transaction.Amount);

                await UnitOfWork.WalletRepository.Update(wallet);
                await UnitOfWork.TransactionRepository.Insert(transaction);
                await UnitOfWork.SaveChangesAsync();
                await UnitOfWork.CommitTransactionAsync();

                return new WalletDTO(wallet);
            }
            catch (Exception e)
            {
                await UnitOfWork.RollbackTransactionAsync();

                throw e;
            }
        }

        /// <summary>
        /// Transfers money from one wallet to another
        /// </summary>
        /// <param name="jmbg"></param>
        /// <param name="relatedJmbg"></param>
        /// <param name="pass"></param>
        /// <param name="amount"></param>
        /// <returns>WalletDTO wallet from transfer occured</returns>
        public async Task<WalletDTO> Transfer(string jmbg,
            string relatedJmbg,
            string pass,
            decimal amount)
        {
            try
            {
                await UnitOfWork.BeginTransactionAsync();
                //check wallets
                var walletFrom = await UnitOfWork.WalletRepository.GetFirstOrDefaultWithIncludes(w => w.JMBG == jmbg);
                ValidateWallet(walletFrom, jmbg, pass);
                if (walletFrom.IsBlocked) throw new WalletServiceException("Vas wallet je blokiran!", "Transfer: Wallet is blocked!");

                var walletTo = await UnitOfWork.WalletRepository.GetFirstOrDefaultWithIncludes(w => w.JMBG == relatedJmbg);
                if (walletTo == null) throw new WalletServiceException("Destinacioni Wallet nije pronadjen!", "Transfer: Wallet not found!");
                if (walletTo.IsBlocked) throw new WalletServiceException("Destinacioni wallet je blokiran!", "Transfer: Wallet is blocked!");

                TransferDTO transfer;
                if (await IsWalletComissionFree(walletFrom))
                {
                    transfer = TransferFactory.CreateTransferAndUpdateWalletsWithoutComission(walletFrom, walletTo, amount);
                }
                else
                {
                    transfer = TransferFactory.CreateTransferAndUpdateWallets(walletFrom, walletTo, amount);
                }

                await UnitOfWork.WalletRepository.Update(walletFrom);
                await UnitOfWork.WalletRepository.Update(walletTo);
                await UnitOfWork.TransactionRepository.Insert(transfer.TransactionOut);
                await UnitOfWork.TransactionRepository.Insert(transfer.TransactionIn);
                await UnitOfWork.SaveChangesAsync();
                await UnitOfWork.CommitTransactionAsync();

                return new WalletDTO(walletFrom);
            }
            catch (Exception e)
            {
                await UnitOfWork.RollbackTransactionAsync();

                throw e;
            }
        }

        public async Task<WalletDTO> UpdatePass(string pass, string jmbg, string newPass)
        {
            var wallet = await UnitOfWork.WalletRepository.GetFirstOrDefaultWithIncludes(w => w.JMBG == jmbg);
            ValidateWallet(wallet, jmbg, pass);

            wallet.SetPASS(newPass);

            await UnitOfWork.WalletRepository.Update(wallet);
            await UnitOfWork.SaveChangesAsync();

            return new WalletDTO(wallet);
        }

        public async Task<WalletDTO> BlockWallet(string jmbg)
        {
            var wallet = await UnitOfWork.WalletRepository.GetFirstOrDefaultWithIncludes(w => w.JMBG == jmbg);
            if (wallet == null) throw new WalletServiceException("Wallet nije pronadjen!", "Deposit: Wallet not found!");

            wallet.Block();

            await UnitOfWork.WalletRepository.Update(wallet);
            await UnitOfWork.SaveChangesAsync();

            return new WalletDTO(wallet);
        }
        public async Task<WalletDTO> UnBlockWallet(string jmbg)
        {
            var wallet = await UnitOfWork.WalletRepository.GetFirstOrDefaultWithIncludes(w => w.JMBG == jmbg);
            if (wallet == null) throw new WalletServiceException("Wallet nije pronadjen!", "Deposit: Wallet not found!");

            wallet.UnBlock();

            await UnitOfWork.WalletRepository.Update(wallet);
            await UnitOfWork.SaveChangesAsync();

            return new WalletDTO(wallet);
        }

        public async Task<WalletDTO> GetWallet(string jmbg, string pass)
        {
            var wallet = await UnitOfWork.WalletRepository.GetFirstOrDefaultWithIncludes(w => w.JMBG == jmbg);
            ValidateWallet(wallet, jmbg, pass);

            return new WalletDTO(wallet);
        }

        /// <summary>
        /// Checks if wallet is free of comission
        /// </summary>
        /// <param name="wallet"></param>
        /// <returns></returns>
        public async Task<bool> IsWalletComissionFree(Wallet wallet)
        {
            if ((DateTime.Now - wallet.CreatedDate).TotalDays < NumberOfFirstDaysWithoutComission) return true;
            if (!(await UnitOfWork.TransactionRepository.HasAnyTransferThisMonth(wallet.Id))) return true;

            return false;
        }

        /// <summary>
        /// Validates wallet, throws error if wallet is not valid
        /// </summary>
        /// <param name="wallet"></param>
        /// <param name="jmbg"></param>
        /// <param name="pass"></param>
        /// <exception cref="WalletServiceException">if wallet is not valid</exception>
        private void ValidateWallet(Wallet wallet, string jmbg, string pass)
        {
            if (wallet == null) throw new WalletServiceException("Wallet nije pronadjen!", "ValidateWallet: Wallet not found!");
            if (wallet.JMBG != jmbg) throw new WalletServiceException("JMBG nije ipsravan!", "ValidateWallet: Invalid JMBG!");
            if (wallet.PASS != pass) throw new WalletServiceException("PASS nije ipsravan!", "ValidateWallet: Invalid PASS!");
        }
    }
}
