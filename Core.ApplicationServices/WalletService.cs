using Common.EfCoreDataAccess;
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
            var result = await UnitOfWork.WalletRepository.GetFilteredList(m => m.JMBG == walletDTO.JMBG);
            if (result.Count > 0)
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

        public async Task<WalletDTO> GetWallet(string walletId)
        {
            var wallet = await UnitOfWork.WalletRepository.GetById(walletId);

            return new WalletDTO(wallet);
        }
    }
}
