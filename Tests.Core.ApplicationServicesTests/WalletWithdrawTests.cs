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
    public class WalletWithdrawTests
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
        public async Task TestWalletWithdraw()
        {
            //starts with 20000
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

                var result = await walletService.Withdraw(
                    "2609992760003", "111111", 1000);

                var wallet = await _coreUnitOfWork.WalletRepository.GetById(result.Id);

                Assert.AreEqual(19000, wallet.Balance, "Balance doesn't match");

            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }

        [TestMethod]
        public async Task TestWalletWithdrawFailTooMuchMoney()
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

                var result = await walletService.Withdraw(
                    "2609992760003", "111111", 1000001);

                var wallet = await _coreUnitOfWork.WalletRepository.GetById(result.Id);

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

        [TestMethod]
        public async Task TestWalletWithdrawFail_NotEnoughMoney()
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

                var result = await walletService.Withdraw(
                    "2609992760003", "111111", 900000);

                var wallet = await _coreUnitOfWork.WalletRepository.GetById(result.Id);

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
    }
}
