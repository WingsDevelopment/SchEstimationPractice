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
    public class WalletTransferTests
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

        // BOTH WALLETS STARTS WITH 20000
        [TestMethod]
        public async Task TestWalletTransfer()
        {
            await Transfer_FirstTransferNoComission_Success();
            await Transfer_WithFixedComission_Success();
            await Transfer_WithPercentageComission_Success();
            await Transfer_FirstWeekComissionFree_Success();
            await Transfer_NotEnoughMoney_Fail();
            await Transfer_SecondWalletBlocked_Fail();
        }

        public async Task Transfer_FirstTransferNoComission_Success()
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

                var result = await walletService.Transfer(
                    "2609992760004", "2609992760005", "111111", 1000);

                var wallet = await _coreUnitOfWork.WalletRepository.GetById(result.Id);
                var wallet2 = await walletService.GetWallet("2609992760005", "111111");

                Assert.AreEqual(19000, wallet.Balance, "Balance doesn't match");
                Assert.AreEqual(21000, wallet2.Balance, "Balance doesn't match");

            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }

        public async Task Transfer_WithFixedComission_Success()
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

                var result = await walletService.Transfer(
                    "2609992760004", "2609992760005", "111111", 2000);

                var wallet = await _coreUnitOfWork.WalletRepository.GetById(result.Id);
                var wallet2 = await walletService.GetWallet("2609992760005", "111111");

                Assert.AreEqual(16900, wallet.Balance, "Balance doesn't match");
                Assert.AreEqual(23000, wallet2.Balance, "Balance doesn't match");

            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }


        public async Task Transfer_WithPercentageComission_Success()
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

                var result = await walletService.Transfer(
                    "2609992760004", "2609992760005", "111111", 15000);

                var wallet = await _coreUnitOfWork.WalletRepository.GetById(result.Id);
                var wallet2 = await walletService.GetWallet("2609992760005", "111111");

                Assert.AreEqual(400, wallet.Balance, "Balance doesn't match");
                Assert.AreEqual(38000, wallet2.Balance, "Balance doesn't match");

            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }


        public async Task Transfer_FirstWeekComissionFree_Success()
        {
            try
            {
                //Arrange
                WalletService walletService = new WalletService(_coreUnitOfWork,
                    new RakicRaiffeisenBrosBankService(),
                    new PassService(TestConfigurations.PassMin, TestConfigurations.PassMin),
                    "7",
                    new TransferFactory(TestConfigurations.PercentageComissionStartingAmount,
                        TestConfigurations.FixedComission,
                        TestConfigurations.PercentageComission),
                    TestConfigurations.MaxWithdraw, TestConfigurations.MaxDeposit);

                var result = await walletService.Transfer(
                    "2609992760004", "2609992760005", "111111", 400);

                var wallet = await _coreUnitOfWork.WalletRepository.GetById(result.Id);
                var wallet2 = await walletService.GetWallet("2609992760005", "111111");

                Assert.AreEqual(0, wallet.Balance, "Balance doesn't match");
                Assert.AreEqual(38400, wallet2.Balance, "Balance doesn't match");

            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }

        public async Task Transfer_NotEnoughMoney_Fail()
        {
            try
            {
                //Arrange
                WalletService walletService = new WalletService(_coreUnitOfWork,
                    new RakicRaiffeisenBrosBankService(),
                    new PassService(TestConfigurations.PassMin, TestConfigurations.PassMin),
                    "7",
                    new TransferFactory(TestConfigurations.PercentageComissionStartingAmount,
                        TestConfigurations.FixedComission,
                        TestConfigurations.PercentageComission),
                    TestConfigurations.MaxWithdraw, TestConfigurations.MaxDeposit);

                var result = await walletService.Transfer(
                    "2609992760004", "2609992760005", "111111", 1000);

                Assert.Fail("Expected error not thrown");

            }
            catch (WalletEntityException ex)
            {
                Assert.IsTrue(ex is WalletEntityException);
            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }

        public async Task Transfer_SecondWalletBlocked_Fail()
        {
            try
            {
                //Arrange
                WalletService walletService = new WalletService(_coreUnitOfWork,
                    new RakicRaiffeisenBrosBankService(),
                    new PassService(TestConfigurations.PassMin, TestConfigurations.PassMin),
                    "7",
                    new TransferFactory(TestConfigurations.PercentageComissionStartingAmount,
                        TestConfigurations.FixedComission,
                        TestConfigurations.PercentageComission),
                    TestConfigurations.MaxWithdraw, TestConfigurations.MaxDeposit);

                await walletService.BlockWallet("2609992760005");

                var result = await walletService.Transfer(
                    "2609992760004", "2609992760005", "111111", 1000);

                Assert.Fail("Expected error not thrown");
            }
            catch (WalletServiceException ex)
            {
                Assert.IsTrue(ex is WalletServiceException);
            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }
    }
}