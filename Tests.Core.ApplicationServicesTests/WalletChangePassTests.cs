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
    public class WalletChangePassTests
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
        public async Task WalletChangePassTests_Success()
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
                        JMBG = "1609992768888",
                        Bank = "Raiffaisen",
                        PIN = "0101",
                        BankAccount = "456-789-123"
                    }
                    );

                await walletService.UpdatePass(result.PASS, result.JMBG, "111111");

                var wallet  = await _coreUnitOfWork.WalletRepository.GetById(result.Id);
                Assert.AreNotEqual(null, wallet, "Wallet must not be null");
                Assert.AreEqual("111111", wallet.PASS, "Wallet must not be null");
            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }

    }
}