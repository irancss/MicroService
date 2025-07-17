using ProductService.Domain.Models;
using Sieve.Models;


namespace ProductService.Domain.Interfaces
{
    public interface IBrandRepository
    {
        Task<Brand> GetByIdAsync(string id);
        Task<IEnumerable<Brand>> GetAllAsync();
        Task<Brand> AddAsync(Brand brand);
        Task UpdateAsync(Brand brand);
        Task DeleteAsync(string id);
        Task<IEnumerable<Brand>> GetPagedAndSortedAsync(SieveModel sieveModel);
    }

}
