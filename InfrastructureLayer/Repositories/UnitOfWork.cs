using ApplicationLayer.Interfaces;
using ApplicationLayer.Persistance;
using DomainLayer;
using InfrastructureLayer.Persistance;
using Microsoft.EntityFrameworkCore.Storage;

namespace InfrastructureLayer.Repositories
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
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Disposes the database context if it hasn't been disposed yet.
        /// Prevents resource leaks and ensures proper garbage collection.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed && _context != null)
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

                if (result < 1)
                    return Result<bool>.Fail();
                
                return Result<bool>.Succeed(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Starts a new database transaction asynchronously.
        /// </summary>
        /// <remarks>
        /// This method ensures that only one transaction is active at a time.
        /// If a transaction already exists, the method returns immediately without creating a new one.
        /// </remarks>
        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null)
                return;

            _currentTransaction = await _context.Database.BeginTransactionAsync();
        }

        /// <summary>
        /// Commits the current database transaction asynchronously.
        /// </summary>
        /// <remarks>
        /// If the commit operation fails, the transaction is rolled back automatically.
        /// After committing, the transaction object is disposed and cleared from memory.
        /// </remarks>
        /// <exception cref="Exception">
        /// Thrown when the commit fails and rollback is executed.
        /// </exception>
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

        /// <summary>
        /// Rolls back the current database transaction asynchronously.
        /// </summary>
        /// <remarks>
        /// This method reverts all changes made within the current transaction.
        /// Regardless of whether rollback succeeds or fails,
        /// the transaction object is disposed and cleared to ensure resource cleanup.
        /// </remarks>
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
