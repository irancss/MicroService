using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using BuildingBlocks.Abstractions;
using BuildingBlocks.Common;

namespace BuildingBlocks.Infrastructure;

public class RepositoryReadOnly<T> : BaseRepository<T>, IRepositoryReadOnly<T> where T : class
{
    public RepositoryReadOnly(DbContext context) : base(context)
    {
    }

    public IPaginate<TResult> GetList<TResult>(Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
        int index = 0, int size = 20) where TResult : class
    {
        return base.GetList(selector, predicate, orderBy, include, index, size, false);
    }

}