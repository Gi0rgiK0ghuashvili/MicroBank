using DomainLayer;
using System.Linq.Expressions;

namespace ApplicationLayer.Interfaces
{
    public interface IGenericRepository<T>
    {
        public Task<Result> AddAsync(T entity);
        public Task<Result> UpdateAsync(T entity);
        public Task<Result> DeleteAsync(T entity);
        public Task<Result<T>> GetByIdAsync(Guid id);

        public Task<Result<T>> GetByExpressionAsync(
            Expression<Func<T, bool>> expression, 
            string? includes = null, 
            bool trackChanges = false);

        public Task<Result<IEnumerable<T>>> ListAsync(
            Expression<Func<T, bool>>? expression = null, 
            string? includes = null, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, 
            int count = 0, 
            bool trachChanges = false);
    }
}
