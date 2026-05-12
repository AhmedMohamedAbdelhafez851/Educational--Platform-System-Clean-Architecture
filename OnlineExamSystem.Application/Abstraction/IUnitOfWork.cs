using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace OnlineExamSystem.Application.Abstraction
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        IRepository<T> Repository<T>() where T : class;
        DbContext GetContext();
        Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        void Commit();
        void Rollback();
    }
}