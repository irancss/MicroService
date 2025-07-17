using Microsoft.EntityFrameworkCore;
using Shopping.SharedKernel.Domain.Entities;

namespace Shopping.SharedKernel.Abstractions;

public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
{
    TContext Context { get; }
}

public interface IUnitOfWork : IDisposable
{
    IRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity;
    IRepositoryAsync<TEntity> GetRepositoryAsync<TEntity>() where TEntity : class;
    IRepositoryReadOnly<TEntity> GetReadOnlyRepository<TEntity>() where TEntity : class;
    IRepositoryReadOnlyAsync<TEntity> GetReadOnlyRepositoryAsync<TEntity>() where TEntity : class;
    IDeleteRepository<TEntity> DeleteRepository<TEntity>() where TEntity : class;

    int Commit();
    Task<int> CommitAsync();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}