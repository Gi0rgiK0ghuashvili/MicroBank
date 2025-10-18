using DomainLayer;

namespace ApplicationLayer.Interfaces
{
    public interface IUnitOfWork
    {
        public Task<Result<bool>> SaveChangesAsync();
    }
}
