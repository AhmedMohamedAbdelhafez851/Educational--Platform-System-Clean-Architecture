using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Infrastructure.Persistence;
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

        public IQueryable<T> GetQueryable() => _dbSet.AsQueryable();

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task AddAsyncRange(IEnumerable<T> entities) => await _dbSet.AddRangeAsync(entities);

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

        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public async Task<IQueryable<T>> GetAllIncludingAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
                query = query.Include(include);
            return await Task.FromResult(query);
        }

        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = true,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, int? skip = null, int? take = null)
        {
            IQueryable<T> query = _dbSet.Where(predicate);
            if (asNoTracking) query = query.AsNoTracking();
            if (orderBy != null) query = orderBy(query);
            if (skip.HasValue) query = query.Skip(skip.Value);
            if (take.HasValue) query = query.Take(take.Value);
            return await query.ToListAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) => await _dbSet.AsNoTracking().AnyAsync(predicate);

        public async Task<int> GetCountAsync(Expression<Func<T, bool>> predicate) => await _dbSet.AsNoTracking().CountAsync(predicate);

        public async Task<IQueryable<T>> GetAllWithNestedIncludesAsync(Func<IQueryable<T>, IQueryable<T>> includeFunc)
        {
            IQueryable<T> query = _dbSet;
            query = includeFunc(query);
            return await Task.FromResult(query);
        }
    }
}