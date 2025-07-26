using BuildingBlocks.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace  BuildingBlocks.Abstractions;

public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
{
    TContext Context { get; }
}

public interface IUnitOfWork : IDisposable
{
    //IRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity;
    //IRepositoryAsync<TEntity> GetRepositoryAsync<TEntity>() where TEntity : class;
    //IRepositoryReadOnly<TEntity> GetReadOnlyRepository<TEntity>() where TEntity : class;
    //IRepositoryReadOnlyAsync<TEntity> GetReadOnlyRepositoryAsync<TEntity>() where TEntity : class;
    //IDeleteRepository<TEntity> DeleteRepository<TEntity>() where TEntity : class;

    //int Commit();
    //Task<int> CommitAsync();
    //Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);


    /// <summary>
    /// Asynchronously saves all changes made in this context to the database.
    /// This method will also dispatch domain events.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

