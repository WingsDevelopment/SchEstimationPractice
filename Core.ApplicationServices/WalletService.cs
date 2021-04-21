﻿using Common.EfCoreDataAccess;
using Core.ApplicationServices.DTOs;
using Core.ApplicationServices.Exceptions;
using Core.Domain.Entities;
using Core.Domain.Repositories;
using Core.Domain.Services.External;
using Core.Domain.Services.Internal;
using Core.Domain.Services.Internal.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.ApplicationServices
{
    public class WalletService
    {
        private readonly IPassService PassService;
        private readonly ICoreUnitOfWork UnitOfWork;
        private readonly IBankService BankService;

        public WalletService(
                   ICoreUnitOfWork unitOfWork,
                   IBankService rakicRaiffeisenBrosBankService,
                   IPassService passService
               )
        {
            UnitOfWork = unitOfWork;
            BankService = rakicRaiffeisenBrosBankService;
            PassService = passService;
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

        public async Task<WalletDTO> GetWallet(string jmbg, string pass)
        {
            var wallet = await UnitOfWork.WalletRepository.GetFirstOrDefaultWithIncludes(w => w.JMBG == jmbg);
            ValidateWallet(wallet, jmbg, pass);

            return new WalletDTO(wallet);
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
            if (wallet == null) new WalletServiceException("Wallet nije pronadjen!", "Deposit: Wallet not found!");
            if (wallet.JMBG != jmbg) new WalletServiceException("JMBG nije ipsravan!", "Deposit: Invalid JMBG!");
            if (wallet.PASS != pass) new WalletServiceException("PASS nije ipsravan!", "Deposit: Invalid PASS!");
        }
    }
}
