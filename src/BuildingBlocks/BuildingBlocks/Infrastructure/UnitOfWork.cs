//using Microsoft.EntityFrameworkCore;
//using BuildingBlocks.Abstractions;
//using BuildingBlocks.Domain.Entities;


//namespace BuildingBlocks.Infrastructure
//{
//    public class UnitOfWork<TContext> : IRepositoryFactory, IUnitOfWork<TContext>
//        where TContext : DbContext, IDisposable
//    {
//        private Dictionary<(Type type, string name), object> _repositories;

//        public UnitOfWork(TContext context)
//        {
//            Context = context ?? throw new ArgumentNullException(nameof(context));
//        }

//        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity
//        {
//            return (IRepository<TEntity>)GetOrAddRepository(typeof(TEntity), new Repository<TEntity>(Context));
//        }

//        public IRepositoryAsync<TEntity> GetRepositoryAsync<TEntity>() where TEntity : class
//        {
//            return (IRepositoryAsync<TEntity>)GetOrAddRepository(typeof(TEntity),
//                new RepositoryAsync<TEntity>(Context));
//        }

//        public IRepositoryReadOnly<TEntity> GetReadOnlyRepository<TEntity>() where TEntity : class
//        {
//            return (IRepositoryReadOnly<TEntity>)GetOrAddRepository(typeof(TEntity),
//                new RepositoryReadOnly<TEntity>(Context));
//        }

//        public IRepositoryReadOnlyAsync<TEntity> GetReadOnlyRepositoryAsync<TEntity>() where TEntity : class
//        {
//            return (IRepositoryReadOnlyAsync<TEntity>)GetOrAddRepository(typeof(TEntity),
//                new RepositoryReadOnlyAsync<TEntity>(Context));
//        }

//        public IDeleteRepository<TEntity> DeleteRepository<TEntity>() where TEntity : class
//        {
//            return (IDeleteRepository<TEntity>)GetOrAddRepository(typeof(TEntity),
//                new DeleteRepository<TEntity>(Context));
//        }

//        public TContext Context { get; }

//        public int Commit()
//        {

//            return Context.SaveChanges();
//        }

//        public async Task<int> CommitAsync()
//        {
//            return await Context.SaveChangesAsync();
//        }

//        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
//        {
//            return await Context.SaveChangesAsync(cancellationToken);
//        }

//        public void Dispose()
//        {
//            Context?.Dispose();
//        }

//        internal object GetOrAddRepository(Type type, object repo)
//        {
//            _repositories ??= new Dictionary<(Type type, string Name), object>();

//            if (_repositories.TryGetValue((type, repo.GetType().FullName), out var repository)) return repository;
//            _repositories.Add((type, repo.GetType().FullName), repo);
//            return repo;
//        }
//    }
//}

using Microsoft.EntityFrameworkCore;
using BuildingBlocks.Abstractions;

namespace BuildingBlocks.Infrastructure
{
    /// <summary>
    /// [اصلاح شد] پیاده‌سازی ساده شده UnitOfWork بدون قابلیت‌های Repository Factory.
    /// </summary>
    public class UnitOfWork<TContext> : IUnitOfWork<TContext>
        where TContext : DbContext
    {
        public TContext Context { get; }

        public UnitOfWork(TContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // منطق ارسال رویدادهای دامنه می‌تواند اینجا یا در یک interceptor باشد.
            // با توجه به وجود MediatorExtensions، بهتر است در Command Handler قبل از فراخوانی این متد باشد.
            // یا می‌توانیم آن را در یک SaveChangesInterceptor متمرکز کنیم که بهترین راه است.
            return await Context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            Context?.Dispose();
        }
    }
}
// --- END OF FILE BuildingBlocks/Infrastructure/UnitOfWork.cs ---