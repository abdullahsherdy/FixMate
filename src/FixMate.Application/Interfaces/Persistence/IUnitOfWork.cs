using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace FixMate.Application.Interfaces.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync(IDbContextTransaction transaction);
        Task RollbackTransactionAsync(IDbContextTransaction transaction);
    }
} 