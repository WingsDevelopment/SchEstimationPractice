using Common.EfCoreDataAccess;
using Core.Domain.Entities;
using Core.Domain.Repositories;

namespace Core.Infrastructure.DataAccess.EfCoreDataAccess.Repositories
{
    public class WalletRepository : EfCoreRepository<Wallet>, IWalletRepository
    {
        public WalletRepository(CoreEfCoreDbContext dbContext) : base(dbContext)
        {
        }
    }
}
