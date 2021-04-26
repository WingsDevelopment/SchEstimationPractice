using Core.ApplicationServices;
using Core.ApplicationServices.DTOs;
using Core.ApplicationServices.Exceptions;
using Core.ApplicationServices.Factories;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Core.Domain.Repositories;
using Core.Domain.Services.Internal;
using Core.Infrastructure.DataAccess.EfCoreDataAccess;
using Core.Infrastructure.Services.RakicRaiffeisenBrosBankService.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Core.ApplicationServicesTests
{

    [TestClass]
    public class BlockedWalletTests
    {
        private ICoreUnitOfWork _coreUnitOfWork;
        private CoreEfCoreDbContext _dbContext;

        [TestInitialize]
        public void Setup()
        {
            var dbContextFactory = new SampleDbContextFactory();
            _dbContext = dbContextFactory.CreateDbContext(new string[] { });
            _coreUnitOfWork = new CoreEfCoreUnitOfWork(_dbContext);
        }

        [TestCleanup()]
        public async Task Cleanup()
        {
            await _dbContext.DisposeAsync();
            _coreUnitOfWork = null;
        }


        [TestMethod]
        public async Task Block_TryWithdraw_TryDeposit_TryTransfer_Unblock()
        {
            await BlockWallet();
            await TryWalletDeposit_OnBlockedWallet();
            await TryWalletWithdraw_OnBlockedWallet();
            await TryWalletTransfer_FromBlockedWallet();
            await UnBlockWallet();
        }

        public async Task BlockWallet()
        {
            try
            {
                //Arrange
                WalletService walletService = new WalletService(_coreUnitOfWork,
                    new RakicRaiffeisenBrosBankService(),
                    new PassService(TestConfigurations.PassMin, TestConfigurations.PassMin),
                    TestConfigurations.NumberOfDaysBeforeComission,
                    new TransferFactory(TestConfigurations.PercentageComissionStartingAmount,
                        TestConfigurations.FixedComission,
                        TestConfigurations.PercentageComission),
                    TestConfigurations.MaxWithdraw, TestConfigurations.MaxDeposit);

                var result = await walletService.BlockWallet("2609992760000");

                var wallet = await _coreUnitOfWork.WalletRepository.GetById(result.Id);

                Assert.AreNotEqual(null, wallet, "Wallet must not be null");
                Assert.AreEqual(true, wallet.IsBlocked, "Wallet is not blocked");
            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }

        private async Task TryWalletDeposit_OnBlockedWallet()
        {
            //Arrange
            WalletService walletService = new WalletService(_coreUnitOfWork,
            new RakicRaiffeisenBrosBankService(),
            new PassService(TestConfigurations.PassMin, TestConfigurations.PassMin),
                TestConfigurations.NumberOfDaysBeforeComission,
            new TransferFactory(TestConfigurations.PercentageComissionStartingAmount,
                TestConfigurations.FixedComission,
                TestConfigurations.PercentageComission),
                TestConfigurations.MaxWithdraw, TestConfigurations.MaxDeposit);
            try
            {
                var result = await walletService.Deposit(
                    "2609992760000", "111111", 1000);

                Assert.Fail("Expected error not thrown");
            }
            catch (WalletServiceException ex)
            {
                var wallet = await walletService.GetWallet("2609992760000", "111111");
                Assert.AreEqual(20000, wallet.Balance, "Balance doesn't match");
            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }

        public async Task TryWalletWithdraw_OnBlockedWallet()
        {
            //Arrange
            WalletService walletService = new WalletService(_coreUnitOfWork,
                new RakicRaiffeisenBrosBankService(),
                new PassService(TestConfigurations.PassMin, TestConfigurations.PassMin),
                TestConfigurations.NumberOfDaysBeforeComission,
                new TransferFactory(TestConfigurations.PercentageComissionStartingAmount,
                    TestConfigurations.FixedComission,
                    TestConfigurations.PercentageComission),
                TestConfigurations.MaxWithdraw, TestConfigurations.MaxDeposit);

            try
            {
                var result = await walletService.Withdraw(
                    "2609992760000", "111111", 20000);

                Assert.Fail("Expected error not thrown");
            }
            catch (WalletServiceException ex)
            {
                var wallet = await walletService.GetWallet("2609992760000", "111111");
                Assert.AreEqual(20000, wallet.Balance, "Balance doesn't match");
            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }

        public async Task TryWalletTransfer_FromBlockedWallet()
        {
            //Arrange
            WalletService walletService = new WalletService(_coreUnitOfWork,
                new RakicRaiffeisenBrosBankService(),
                new PassService(TestConfigurations.PassMin, TestConfigurations.PassMin),
                TestConfigurations.NumberOfDaysBeforeComission,
                new TransferFactory(TestConfigurations.PercentageComissionStartingAmount,
                    TestConfigurations.FixedComission,
                    TestConfigurations.PercentageComission),
                TestConfigurations.MaxWithdraw, TestConfigurations.MaxDeposit);

            try
            {
                var result = await walletService.Transfer(
                    "2609992760000", "111111", "2609992760001", 20000);

                Assert.Fail("Expected error not thrown");
            }
            catch (WalletServiceException ex)
            {
                var wallet = await walletService.GetWallet("2609992760000", "111111");
                Assert.AreEqual(20000, wallet.Balance, "Balance doesn't match");
            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }

        public async Task UnBlockWallet()
        {
            try
            {
                //Arrange
                WalletService walletService = new WalletService(_coreUnitOfWork,
                    new RakicRaiffeisenBrosBankService(),
                    new PassService(TestConfigurations.PassMin, TestConfigurations.PassMin),
                    TestConfigurations.NumberOfDaysBeforeComission,
                    new TransferFactory(TestConfigurations.PercentageComissionStartingAmount,
                        TestConfigurations.FixedComission,
                        TestConfigurations.PercentageComission),
                    TestConfigurations.MaxWithdraw, TestConfigurations.MaxDeposit);

                var result = await walletService.UnBlockWallet("2609992760000");

                var wallet = await _coreUnitOfWork.WalletRepository.GetById(result.Id);

                Assert.AreNotEqual(null, wallet, "Wallet must not be null");
                Assert.AreEqual(false, wallet.IsBlocked, "Wallet is not unblocked");
            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }

    }
}
