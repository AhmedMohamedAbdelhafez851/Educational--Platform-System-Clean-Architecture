using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Infrastructure.Repositories;

namespace OnlineExamSystem.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();
        private bool _disposed;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);
            if (!_repositories.TryGetValue(type, out var repo))
            {
                repo = new Repository<T>(_context);
                _repositories[type] = repo;
            }
            return (IRepository<T>)repo;
        }

        public DbContext GetContext() => _context;

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
            return _transaction;
        }

        public void Commit()
        {
            _transaction?.Commit();
        }

        public void Rollback()
        {
            _transaction?.Rollback();
            _transaction?.Dispose();
            _transaction = null!;
        }

        // ✅ FIXED: Returns ValueTask, not Task
        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                if (_transaction != null)
                    await _transaction.DisposeAsync();

                if (_context != null)
                    await _context.DisposeAsync();

                _repositories.Clear();
                _disposed = true;
            }
        }

        // ✅ FIXED: Standard Dispose
        public void Dispose()
        {
            if (!_disposed)
            {
                _transaction?.Dispose();
                _context?.Dispose();
                _repositories.Clear();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}