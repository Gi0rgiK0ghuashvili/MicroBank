using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityAbstactions;
using InfrastructureLayer.Persistance;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InfrastructureLayer.Interfaces
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _dbSet = context.Set<T>();
        }

        public async Task<Result> AddAsync(T entity)
        {
            try
            {
                if (entity == null)
                    return Result.Fail("მიღებული არგუმენტი არ შეიძლება იყოს null.", 400);

                var addedEntity = await _dbSet.AddAsync(entity);

                if(addedEntity == null)
                    return Result.Fail("მონაცემების დამატებისასა დაფიქსირდა შეცდომა.", 500);

                else
                    return Result.Succeed("მონაცემები წარმატებით დაემატა.");
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);

            }
        }

        public async Task<Result> DeleteAsync(T entity)
        {
            try
            {
                if (entity == null)
                    return Result.Fail("მიღებული არგუმენტი არ შეიძლება იყოს null.", 400);

                var removableEntity = await _dbSet.FirstOrDefaultAsync(x => x.Id == entity.Id);

                if(removableEntity == null)
                    return Result.Fail($"მოთხოვნილი ობიექტის Id-ი ბაზაში ვერ მოიძებნა. Id:{entity.Id}", 400);

                if(!removableEntity.Active)
                    return Result.Fail($"მოთხოვნილი ობიექტის Id-ით უკვე წაშლილია ბაზაში. Id:{entity.Id}", 400);

                removableEntity.Active = false;

                return Result.Succeed("მონაცემები წარმატებით წაიშალა.");
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        public async Task<Result<T>> GetByExpressionAsync(Expression<Func<T, bool>> expression, string? includes = null, bool trackChanges = false)
        {
            if (_dbSet == null)
                return Result<T>.Fail("Database is null!");

            if (expression == null)
                return Result<T>.Fail("Expression is null!");

            IQueryable<T> query = _dbSet;

            if (includes != null)
            {
                var includesList = includes.Split(", ").ToList();

                if (includesList != null)
                    query = includesList.Aggregate(query, (current, includesList) => current.Include(includesList));
            }

            var item = trackChanges ? await query.FirstOrDefaultAsync(expression) : await query.AsNoTracking().FirstOrDefaultAsync(expression);

            if (item != null)
                return Result<T>.Succeed(item);
            else
                return Result<T>.Fail();
        }

        public async Task<Result> GetByIdAsync(int id)
        {
            try
            {
                if (id == null)
                    return Result.Fail("მიღებული არგუმენტი არ შეიძლება იყოს null.", 400);

                if (id <= 0)
                    return Result.Fail("მიღებული id-ი უნდა იყოს ნულზე მეტი.", 400);

                var addedEntity = await _dbSet.FirstOrDefaultAsync(x => x.Id == id && x.Active == true);

                if (addedEntity == null)
                    return Result.Fail("მონაცემების დამატებისასა დაფიქსირდა შეცდომა.", 500);

                else
                    return Result.Succeed("მონაცემები წარმატებით დაემატა.");
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);

            }
        }

        public async Task<Result<IEnumerable<T>>> ListAsync(Expression<Func<T, bool>>? expression = null, string? includes = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, int count = 0, bool trackChanges = false)
        {
            IQueryable<T> query = _dbSet;

            if (includes != null)
            {
                var includesList = includes.Split(", ").ToList();

                if (includesList != null)
                {
                    query = includesList.Aggregate(query, (current, includesList) => current.Include(includesList));
                }
            }

            if (expression != null)
                query = query.Where(expression);

            if (orderBy != null)
                query = orderBy(query);

            if (count > 0)
                query = query.Take(count);

            var result = trackChanges ? await query.ToListAsync() : await query.AsNoTracking().ToListAsync();

            if (result != null)

                return Result<IEnumerable<T>>.Succeed(result);
            else
                return Result<IEnumerable<T>>.Fail();
        }

        public async Task<Result> UpdateAsync(T entity)
        {
            try
            {
                if (entity == null)
                    return Result.Fail("მიღებული არგუმენტი არ შეიძლება იყოს null.", 400);

                var addedEntity = _dbSet.Update(entity);
                await Task.CompletedTask;
                if (addedEntity == null)
                    return Result.Fail("მონაცემების განახლებისას დაფიქსირდა შეცდომა.", 500);
                else
                    return Result.Succeed("მონაცემები წარმატებით დაემატა.");
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);

            }
        }
    }
}
