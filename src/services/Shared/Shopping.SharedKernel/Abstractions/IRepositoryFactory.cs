using Shopping.SharedKernel.Domain.Entities;

namespace Shopping.SharedKernel.Abstractions;

public interface IRepositoryFactory
{
    IRepository<T> GetRepository<T>() where T : BaseEntity;
    IRepositoryAsync<T> GetRepositoryAsync<T>() where T : class;
    IRepositoryReadOnly<T> GetReadOnlyRepository<T>() where T : class;
}