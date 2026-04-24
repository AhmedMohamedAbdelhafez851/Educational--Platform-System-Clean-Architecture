// BL/Repositories/Repository.cs
using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.BL.Infrastructure.Persistence;
using System.Linq.Expressions;
namespace OnlineExamSystem.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task AddAsyncRange(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        public async Task<T?> GetByIdAsync(int id)
            => await _dbSet.FindAsync(id);

        public async Task<IQueryable<T>> GetAllIncludingAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await Task.FromResult(query);
        }

        // IMPROVED: Optimized GetListAsync with performance features
        public async Task<List<T>> GetListAsync(
            Expression<Func<T, bool>> predicate,
            bool asNoTracking = true,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            int? skip = null,
            int? take = null)
        {
            IQueryable<T> query = _dbSet.Where(predicate);

            // Critical for read-only operations - reduces memory and improves speed
            if (asNoTracking)
                query = query.AsNoTracking();

            // Add sorting for better index utilization
            if (orderBy != null)
                query = orderBy(query);

            // Pagination to reduce data transfer
            if (skip.HasValue)
                query = query.Skip(skip.Value);
            if (take.HasValue)
                query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        // NEW: Efficient count without loading data
        public async Task<int> GetCountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().CountAsync(predicate);
        }

        // NEW: Fast existence check (stops at first match)
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().AnyAsync(predicate);
        }

        public async Task<IQueryable<T>> GetAllWithNestedIncludesAsync(Func<IQueryable<T>, IQueryable<T>> includeFunc)
        {
            IQueryable<T> query = _dbSet;
            query = includeFunc(query);
            return await Task.FromResult(query);
        }

        // OPTIONAL: Batch operations for multiple predicates
        public async Task<Dictionary<bool, List<T>>> GetListPartitionedAsync(
            params (Expression<Func<T, bool>> Predicate, string Key)[] predicates)
        {
            var results = new Dictionary<bool, List<T>>();
            var query = _dbSet.AsNoTracking();

            foreach (var (predicate, key) in predicates)
            {
                var items = await query.Where(predicate).ToListAsync();
                results[bool.Parse(key)] = items;
            }

            return results;
        }
    }
}