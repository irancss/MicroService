using BuildingBlocks.Abstractions;
using CustomerService.Domain.Entities;

namespace CustomerService.Domain.Interfaces
{
    public interface ICustomerRepository 
    {
        Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Customer?> GetByMobileAsync(string email, CancellationToken cancellationToken = default);
        Task<Customer?> GetByAddressAsync(string email, CancellationToken cancellationToken = default);
        Task<Customer?> GetByCityAsync(string email, CancellationToken cancellationToken = default);

        Task<Guid> InsertAsync(Customer customer, CancellationToken cancellationToken = default);
    }
}
