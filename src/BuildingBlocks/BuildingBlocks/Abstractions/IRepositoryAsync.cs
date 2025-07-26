using System.Linq.Expressions;
using BuildingBlocks.Common;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;

namespace  BuildingBlocks.Abstractions;

public interface IRepositoryAsync<T> where T : class
{
    Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy);

    Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> include);

    Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> include, bool enableTracking);

    Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
        bool enableTracking = true,
        bool ignoreQueryFilters = false);

    Task<IPaginate<T>> GetListAsync(Expression<Func<T, bool>> predicate);
    Task<IPaginate<T>> GetListAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy);
    Task<IPaginate<T>> GetListAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, Func<IQueryable<T>, IIncludableQueryable<T, object>> include);
    Task<IPaginate<T>> GetListAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, Func<IQueryable<T>, IIncludableQueryable<T, object>> include, int index,
        int size);
    Task<IPaginate<T>> GetListAsync(Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
        int index = 0,
        int size = 20,
        bool enableTracking = true,
        CancellationToken cancellationToken = default);

    #region Insert Functions

    ValueTask<EntityEntry<T>> InsertAsync(T entity,
        CancellationToken cancellationToken = default);

    Task InsertAsync(params T[] entities);

    Task InsertAsync(IEnumerable<T> entities,
        CancellationToken cancellationToken = default);

    #endregion
}