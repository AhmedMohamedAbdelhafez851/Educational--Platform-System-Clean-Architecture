using Microsoft.EntityFrameworkCore.Storage; // For IDbContextTransaction
using Microsoft.EntityFrameworkCore;

namespace OnlineExamSystem.Application.Abstraction
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> Repository<T>() where T : class;
        Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        void Commit();
        void Rollback();
        Task DisposeAsync(); // Changed from Task ValueTask to Task for simplicity
        DbContext GetContext(); // Add this method
    }
}