using System.Linq.Expressions;

namespace OnlineExamSystem.Application.Abstraction
{
    public interface IRepository<T> where T : class
    {
        Task<T> AddAsync(T entity);
        Task AddAsyncRange(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<T?> GetByIdAsync(int id);
        Task<IQueryable<T>> GetAllIncludingAsync(params Expression<Func<T, object>>[] includes);
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = true, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, int? skip = null, int? take = null);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<int> GetCountAsync(Expression<Func<T, bool>> predicate);
        Task<IQueryable<T>> GetAllWithNestedIncludesAsync(Func<IQueryable<T>, IQueryable<T>> includeFunc);
        IQueryable<T> GetQueryable();
    }
}