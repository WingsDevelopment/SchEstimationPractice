using Common.EfCoreDataAccess;
using Core.Domain.Repositories;
using Core.Infrastructure.DataAccess.EfCoreDataAccess.Repositories;

namespace Core.Infrastructure.DataAccess.EfCoreDataAccess
{
    public class CoreEfCoreUnitOfWork : EfCoreUnitOfWork, ICoreUnitOfWork
    {
        public IWalletRepository WalletRepository { get; }

        public ITransactionRepository TransactionRepository { get; }

        public CoreEfCoreUnitOfWork(CoreEfCoreDbContext context) : base(context)
        {
            WalletRepository = new WalletRepository(context);
            TransactionRepository = new TransactionRepository(context);
        }
    }
}
