using DomainLayer;

namespace ApplicationLayer.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        public Task<Result<bool>> SaveChangesAsync();
        public void Dispose();

        public Task BeginTransactionAsync();
        public Task CommitTransactionAsync();
        public Task RollbackTransactionAsync();
    }
}
