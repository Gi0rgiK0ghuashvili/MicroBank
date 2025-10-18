using ApplicationLayer.Interfaces;
using ApplicationLayer.Persistance;
using DomainLayer;

namespace InfrastructureLayer.Interfaces
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool _disposed;

        private readonly IApplicationDbContext _context;

        public UnitOfWork(IApplicationDbContext context)
        {
            _context = context;
        }

        public void Dispose()
        {
            if (_disposed = false && _context != null)
            {
                _context.Dispose();
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        public async Task<Result<bool>> SaveChangesAsync()
        {
            try
            {
                if (_context == null)
                    return Result<bool>.Fail("Context is null.");

                var result = await _context.SaveChangesAsync();

                if (result >= 1)
                    return Result<bool>.Succeed(true);
                else
                    return Result<bool>.Fail();
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail(ex.Message);
            }
        }
    }
}
