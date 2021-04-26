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
    public class CreateWalletTests
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
        public async Task TestCreateWallet()
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
                        JMBG = "1609992768014",
                        Bank = "Raiffaisen",
                        PIN = "0101",
                        BankAccount = "456-789-123"
                    }
                    );

                var wallet = await _coreUnitOfWork.WalletRepository.GetById(result.Id);

                Assert.AreNotEqual(null, wallet, "Wallet must not be null");
                Assert.AreEqual("Srdjan", wallet.FirstName, "Firstname doesn't match");
                Assert.AreEqual("Rakic", wallet.LastName, "LastName doesn't match");
                Assert.AreEqual("1609992768014", wallet.JMBG, "JMBG doesn't match");
                Assert.AreEqual("Raiffaisen", wallet.Bank, "Bank doesn't match");
                Assert.AreEqual("0101", wallet.PIN, "PIN doesn't match");
                Assert.AreEqual("456-789-123", wallet.BankAccount, "BankAccount doesn't match");
            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }

        [TestMethod]
        public async Task CreateWalletFailJMBGNotAdolt()
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
                        JMBG = "3112020768014",
                        Bank = "Raiffaisen",
                        PIN = "0101",
                        BankAccount = "456-789-123"
                    }
                    );

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

        [TestMethod]
        public async Task CreateWalletFailJMBGNotSrb()
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
                        JMBG = "1609992268014",
                        Bank = "Raiffaisen",
                        PIN = "0101",
                        BankAccount = "456-789-123"
                    }
                );

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
