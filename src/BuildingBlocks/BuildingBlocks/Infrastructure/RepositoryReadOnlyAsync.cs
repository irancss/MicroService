using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using BuildingBlocks.Abstractions;
using BuildingBlocks.Common;

namespace BuildingBlocks.Infrastructure;

public class RepositoryReadOnlyAsync<T> : RepositoryAsync<T>, IRepositoryReadOnlyAsync<T> where T : class
{
    public RepositoryReadOnlyAsync(DbContext context) : base(context)
    {
    }

    public async Task<IPaginate<TResult>> GetListAsync<TResult>(Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
        int index = 0,
        int size = 20,
        CancellationToken cancellationToken = default,
        bool ignoreQueryFilters = false) where TResult : class
    {
        return await base.GetListAsync(selector, predicate, orderBy, include, index, size, false, cancellationToken,
            ignoreQueryFilters);
    }
}