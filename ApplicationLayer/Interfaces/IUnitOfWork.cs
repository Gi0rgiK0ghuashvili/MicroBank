using DomainLayer;

namespace ApplicationLayer.Interfaces
{
    public interface IUnitOfWork
    {
        public Task<Result<bool>> SaveChangesAsync();

        public Task BeginTransactionAsync();
        public Task CommitTransactionAsync();
        public Task RollbackTransactionAsync();
    }
}
