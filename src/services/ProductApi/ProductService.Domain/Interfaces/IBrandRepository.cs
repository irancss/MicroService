using BuildingBlocks.Abstractions;
using ProductService.Domain.Models;
using Sieve.Models;


namespace ProductService.Domain.Interfaces
{
    public interface IBrandRepository : IRepositoryAsync<Brand>
    {
        Task<Brand?> GetByIdAsync(Guid brandId);
        Task<IEnumerable<Brand>> GetAllAsync();
        Task AddAsync(Brand brand);
        Task DeleteAsync(Brand brand);
    }

}
