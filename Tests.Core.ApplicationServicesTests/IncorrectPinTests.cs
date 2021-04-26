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
using Core.Infrastructure.Services.RakicRaiffeisenBrosBankService.Mock.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Core.ApplicationServicesTests
{
    [TestClass]
    public class IncorrectPinTests
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
        public async Task WalletWithdraw_IncorrectPIN_Fail()
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
                    "2609992760006", "111111", 1000);

                Assert.Fail("Expected error not thrown");
            }
            catch (RakicRaiffeisenBrosException ex)
            {
                _coreUnitOfWork.ClearTracking();
                var wallet = await walletService.GetWallet("2609992760007", "111111");
                Assert.AreEqual(20000, wallet.Balance, "Balance doesn't match");
            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }

        [TestMethod]
        public async Task WalletDeposit_IncorrectPIN_Fail()
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
                    "2609992760007", "111111", 1000);

                Assert.Fail("Expected error not thrown");
            }
            catch (RakicRaiffeisenBrosException ex)
            {
                _coreUnitOfWork.ClearTracking();
                var wallet = await walletService.GetWallet("2609992760007", "111111");
                Assert.AreEqual(20000, wallet.Balance, "Balance doesn't match");
            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }

        [TestMethod]
        public async Task CreateWalletFail_IncorrectPIN()
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

                var result = await walletService.CreateWallet(
                    new WalletDTO()
                    {
                        FirstName = "Srdjan",
                        LastName = "Rakic",
                        JMBG = "1609992768015",
                        Bank = "Raiffaisen",
                        PIN = "0000",
                        BankAccount = "456-789-123"
                    }
                    );

                Assert.Fail("Expected error not thrown");
            }
            catch (RakicRaiffeisenBrosException ex)
            {
                Assert.IsTrue(ex is RakicRaiffeisenBrosException);
            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }
    }
}