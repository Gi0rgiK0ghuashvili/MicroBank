using ApplicationLayer.Interfaces;
using ApplicationLayer.Persistance;
using DomainLayer;
using Microsoft.EntityFrameworkCore.Storage;

namespace InfrastructureLayer.Interfaces
{
    /// <summary>
    /// Implements the Unit of Work pattern to manage database transactions and context lifecycle.
    /// Provides a centralized way to commit changes and dispose of the database context.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private bool _disposed;

        private readonly IApplicationDbContext _context;
        private IDbContextTransaction _currentTransaction;
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public UnitOfWork(IApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Disposes the database context if it hasn't been disposed yet.
        /// Prevents resource leaks and ensures proper garbage collection.
        /// </summary>
        public void Dispose()
        {
            if (_disposed = false && _context != null)
            {
                _context.Dispose();
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Asynchronously commits all pending changes to the database context.
        /// </summary>
        /// <returns>A Result indicating success (true) or failure, along with an optional error message.</returns>
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

        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null)
                return;

            _currentTransaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _currentTransaction?.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                await _currentTransaction?.RollbackAsync();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }
    }
}
