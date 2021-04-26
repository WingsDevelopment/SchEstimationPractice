using Core.ApplicationServices;
using Core.ApplicationServices.DTOs;
using Core.ApplicationServices.Exceptions;
using Core.ApplicationServices.Factories;
using Core.Domain.Entities;
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
    public class WalletDepositTests
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
        public async Task TestWalletDeposit()
        {
            await DepositSuccess();
            await DepositFromWalletFail_TooMuchMoney();
        }

        public async Task DepositSuccess()
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


                var result = await walletService.Deposit(
                    "2609992760002", "111111", 20000);

                var wallet = await _coreUnitOfWork.WalletRepository.GetById(result.Id);

                Assert.AreEqual(20000, wallet.Balance, "Balance doesn't match");

            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }

        public async Task DepositFromWalletFail_TooMuchMoney()
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

                var result = await walletService.Deposit(
                    "2609992760002", "111111", 1000001);

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
