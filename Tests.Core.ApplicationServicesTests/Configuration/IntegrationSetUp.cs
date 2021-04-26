using Core.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Core.ApplicationServicesTests
{
    [TestClass]
    public class IntegrationSetUp
    {
        [AssemblyInitialize()]
        public static async Task AssemblyInit(TestContext context)
        {
            var dbContextFactory = new SampleDbContextFactory();
            using (var dbContext = dbContextFactory.CreateDbContext(new string[] { }))
            {
                await dbContext.Database.EnsureCreatedAsync();
                dbContext.Database.BeginTransaction();

                //BLOCK TESTS DATA
                Wallet walletForBlockTests1 = new Wallet("BlockWalletTests1", "BlockWalletTests1",
                    "2609992760000", "Raifaisen",
                    "0101", "01-54645-01", "111111", 20000);
                dbContext.Wallets.Add(walletForBlockTests1);
                dbContext.SaveChanges();
                Wallet walletForBlockTests2 = new Wallet("BlockWalletTests2", "BlockWalletTests2",
                    "2609992760001", "Raifaisen",
                    "0101", "01-54645-01", "111111", 20000);
                dbContext.Wallets.Add(walletForBlockTests2);
                dbContext.SaveChanges();

                //DEPOSIT TESTS DATA
                Wallet walletForDepositTests1 = new Wallet("WalletDepositTests", "WalletDepositTests",
                    "2609992760002", "Raifaisen",
                    "0101", "01-54645-01", "111111");
                dbContext.Wallets.Add(walletForDepositTests1);
                dbContext.SaveChanges();

                //WITHDRAW TESTS DATA
                Wallet walletForWithdrawTests1 = new Wallet("WalletWithdrawTests", "WalletWithdrawTests",
                    "2609992760003", "Raifaisen",
                    "0101", "01-54645-01", "111111", 20000);
                dbContext.Wallets.Add(walletForWithdrawTests1);
                dbContext.SaveChanges();

                //TRANSFER TESTS DATA
                Wallet walletForTransferTests1 = new Wallet("WalletTRANSFERTests1", "WalletTRANSFERTests1",
                    "2609992760004", "Raifaisen",
                    "0101", "01-54645-01", "111111", 20000);
                dbContext.Wallets.Add(walletForTransferTests1);
                dbContext.SaveChanges();
                Wallet walletForTransferTests2 = new Wallet("WalletTRANSFERTests2", "WalletTRANSFERTests2",
                    "2609992760005", "Raifaisen",
                    "0101", "01-54645-01", "111111", 20000);
                dbContext.Wallets.Add(walletForTransferTests2);
                dbContext.SaveChanges();

                //PIN ERROR TESTS DATA
                Wallet walletForPinErrorTests1 = new Wallet("walletForPinErrorTests1", "walletForPinErrorTests1",
                    "2609992760006", "Raifaisen",
                    "0001", "01-54645-01", "111111", 20000);
                dbContext.Wallets.Add(walletForPinErrorTests1);
                dbContext.SaveChanges();
                Wallet walletForPinErrorTests2 = new Wallet("walletForPinErrorTests2", "walletForPinErrorTests2",
                    "2609992760007", "Raifaisen",
                    "0002", "01-54645-01", "111111", 20000);
                dbContext.Wallets.Add(walletForPinErrorTests2);
                dbContext.SaveChanges();


                dbContext.SaveChanges();
                dbContext.Database.CommitTransaction();
            }
        }

        [AssemblyCleanup()]
        public static async Task AssemblyCleanup()
        {
            var dbContextFactory = new SampleDbContextFactory();
            using (var dbContext = dbContextFactory.CreateDbContext(new string[] { }))
            {
                await dbContext.Database.EnsureDeletedAsync();
            }
        }
    }
}
