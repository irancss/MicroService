using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Shopping.SharedKernel.Common;

namespace Shopping.SharedKernel.Abstractions;

public interface IRepositoryReadOnlyAsync<T> where T : class
{
    Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null
    );
        
    Task<TResult> SingleOrDefaultAsync<TResult>(Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false);

    Task<IPaginate<T>> GetListAsync(Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
        int index = 0,
        int size = 20);


    Task<IPaginate<TResult>> GetListAsync<TResult>(Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
        int index = 0,
        int size = 20,
        CancellationToken cancellationToken = default,
        bool ignoreQueryFilters = false)
        where TResult : class;
}