using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OnlineExamSystem.Infrastructure.Repositories;
using OnlineExamSystem.Application.Abstraction;

namespace OnlineExamSystem.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();
        private bool _disposed;
        private IDbContextTransaction _transaction;

        // ✅ SINGLE CONSTRUCTOR - remove the duplicate
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // ✅ Optional: Full constructor for dependency injection
        public UnitOfWork(ApplicationDbContext context, Dictionary<Type, object> repositories, IDbContextTransaction transaction)
            : this(context)  // Chain to main constructor
        {
            if (repositories != null)
            {
                foreach (var repo in repositories)
                {
                    _repositories[repo.Key] = repo.Value;
                }
            }
            _transaction = transaction;
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

        public async Task DisposeAsync()
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

        public void Dispose()
        {
            if (!_disposed)
            {
                _transaction?.Dispose();
                _context?.Dispose();  // ✅ Added null check
                _repositories.Clear();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }

      
    }
}