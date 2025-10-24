using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityAbstactions;
using InfrastructureLayer.Persistance;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InfrastructureLayer.Repositories
{
    /// <summary>
    /// A generic repository implementation for performing CRUD operations on entities.
    /// Uses Entity Framework Core's DbSet to interact with the database.
    /// </summary>
    /// <typeparam name="T">The type of the entity, which must inherit from BaseEntity.</typeparam>
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly DbSet<T> _dbSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRepository{T}"/> class.
        /// </summary>
        /// <param name="context">The application's database context.</param>
        public GenericRepository(ApplicationDbContext context)
        {
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Asynchronously adds a new entity to the database.
        /// </summary>
        /// <param name="entity">The entity to be added.</param>
        /// <returns>A Result indicating success or failure with a message and optional status code.</returns>
        public async Task<Result> AddAsync(T entity)
        {
            try
            {
                if (entity == null)
                    return Result.Fail("The provided argument cannot be null.", 400);

                var addedEntity = await _dbSet.AddAsync(entity);

                if (addedEntity == null)
                    return Result.Fail("An error occurred while adding the entity.", 500);
                
                return Result.Succeed("Entity successfully added.");
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);

            }
        }

        /// <summary>
        /// Asynchronously marks an entity as inactive (soft delete).
        /// </summary>
        /// <param name="entity">The entity to be deleted.</param>
        /// <returns>A Result indicating success or failure with a message and optional status code.</returns>
        public async Task<Result> DeleteAsync(T entity)
        {
            try
            {
                if (entity == null)
                    return Result.Fail("The provided argument cannot be null.", 400);

                var removableEntity = await _dbSet.FirstOrDefaultAsync(x => x.Id == entity.Id);

                if (removableEntity == null)
                    return Result.Fail($"Entity with the given ID not found. Id: {entity.Id}", 400);

                if (!removableEntity.Active)
                    return Result.Fail($"Entity with the given ID is already marked as deleted. Id: {entity.Id}", 400);

                removableEntity.Active = false;
                removableEntity.UpdateDate = DateTime.UtcNow;

                return Result.Succeed("Entity successfully deleted.");
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves a single entity that matches the given expression.
        /// Supports optional includes and tracking behavior.
        /// </summary>
        /// <param name="expression">The filter expression to apply.</param>
        /// <param name="includes">Comma-separated list of navigation properties to include.</param>
        /// <param name="trackChanges">Indicates whether to track changes for the entity.</param>
        /// <returns>A Result containing the found entity or failure info.</returns>
        public async Task<Result<T>> GetByExpressionAsync(Expression<Func<T, bool>> expression, string? includes = null, bool trackChanges = false)
        {
            if (_dbSet == null)
                return Result<T>.Fail("Database is null!");

            if (expression == null)
                return Result<T>.Fail("Expression is null!");

            IQueryable<T> query = _dbSet;

            if (includes != null)
            {
                var includesList = includes.Split(',', StringSplitOptions.TrimEntries).ToList();

                if (includesList != null)
                    query = includesList.Aggregate(query, (current, includesList) => current.Include(includesList));
            }

            var item = trackChanges ? await query.FirstOrDefaultAsync(expression) : await query.AsNoTracking().FirstOrDefaultAsync(expression);

            if (item == null)
                return Result<T>.Fail();
            
            return Result<T>.Succeed(item);
        }

        /// <summary>
        /// Retrieves an entity by its ID if it is active.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns>A Result indicating success or failure with a message.</returns>
        public async Task<Result<T>> GetByIdAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return Result<T>.Fail("The Id cannot be empty.", 400);

                var addedEntity = await _dbSet.FirstOrDefaultAsync(x => x.Id == id && x.Active == true);

                if (addedEntity == null)
                    return Result<T>.Fail("Entity not found.", 404);
                
                return Result<T>.Succeed(addedEntity);
            }
            catch (Exception ex)
            {
                return Result<T>.Fail(ex.Message);

            }
        }

        /// <summary>
        /// Retrieves a list of entities based on optional filtering, ordering, and limiting parameters.
        /// </summary>
        /// <param name="expression">Optional filter expression.</param>
        /// <param name="includes">Comma-separated list of navigation properties to include.</param>
        /// <param name="orderBy">Function to order the query.</param>
        /// <param name="count">Limits the number of records returned.</param>
        /// <param name="trackChanges">Whether to track entity changes.</param>
        /// <returns>A Result containing a list of entities or failure info.</returns>

        public async Task<Result<IEnumerable<T>>> ListAsync(Expression<Func<T, bool>>? expression = null, string? includes = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, int count = 0, bool trackChanges = false)
        {
            IQueryable<T> query = _dbSet;

            if (includes != null)
            {
                var includesList = includes.Split(',', StringSplitOptions.TrimEntries).ToList();

                if (includesList != null)
                    query = includesList.Aggregate(query, (current, include) => current.Include(include));
                
            }

            if (expression != null)
                query = query.Where(expression);

            if (orderBy != null)
                query = orderBy(query);

            if (count > 0)
                query = query.Take(count);

            var result = trackChanges ? await query.ToListAsync() : await query.AsNoTracking().ToListAsync();

            if (result == null)
                return Result<IEnumerable<T>>.Fail();
            
            return Result<IEnumerable<T>>.Succeed(result);
        }

        /// <summary>
        /// Updates an existing entity in the database.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>A Result indicating success or failure with a message.</returns>
        public async Task<Result> UpdateAsync(T entity)
        {
            try
            {
                if (entity == null)
                    return Result.Fail("The provided argument cannot be null.", 400);

                var updatedEntity = _dbSet.Update(entity);

                if (updatedEntity == null)
                    return Result.Fail("An error occurred while updating the entity.", 500);

                return await Task.FromResult(Result.Succeed("Entity successfully updated."));

            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
    }
}
