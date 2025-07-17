using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Shopping.SharedKernel.Common;

namespace Shopping.SharedKernel.Abstractions;

public interface IReadRepository<T> where T : class
{
    T SingleOrDefault(Expression<Func<T, bool>> predicate);
    T SingleOrDefault(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy);
    T SingleOrDefault(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,   Func<IQueryable<T>, IIncludableQueryable<T, object>> include);
        
    T SingleOrDefault(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,   Func<IQueryable<T>, IIncludableQueryable<T, object>> include,  bool enableTracking);

    T SingleOrDefault(Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
        bool enableTracking = true,
        bool ignoreQueryFilters = false);
        
    TResult SingleOrDefault<TResult>(Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false);
        
         

    IPaginate<T> GetList(Expression<Func<T, bool>> predicate);
        
    IPaginate<T> GetList(Expression<Func<T, bool>> predicate,  Func<IQueryable<T>, IOrderedQueryable<T>> orderBy);
        
    IPaginate<T> GetList(Expression<Func<T, bool>> predicate,  Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,  Func<IQueryable<T>, IIncludableQueryable<T, object>> include);
    IPaginate<T> GetList(Expression<Func<T, bool>> predicate,  Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,  Func<IQueryable<T>, IIncludableQueryable<T, object>> include,  int index, int size);

    IPaginate<T> GetList(Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
        int index = 0,
        int size = 20,
        bool enableTracking = true);

    IPaginate<TResult> GetList<TResult>(Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
        int index = 0,
        int size = 20,
        bool enableTracking = true) where TResult : class;
}