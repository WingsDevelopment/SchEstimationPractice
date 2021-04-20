using Common.EfCoreDataAccess;
using Core.Domain.Entities;
using Core.Domain.Repositories;

namespace Core.Infrastructure.DataAccess.EfCoreDataAccess.Repositories
{
    public class TransactionRepository : EfCoreRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(CoreEfCoreDbContext dbContext) : base(dbContext)
        {
        }
    }
}
