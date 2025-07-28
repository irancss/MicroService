using ProductService.Application.DTOs;
using ProductService.Application.DTOs.Brand;
using Sieve.Models;

namespace ProductService.Application.Interfaces
{
    public interface IBrandService
    {
        Task<PaginatedList<BrandListDto>> GetAllBrandsAsync(SieveModel sieveModel);
        Task<BrandDto> GetBrandByIdAsync(Guid id);
        Task<string> CreateBrandAsync(BrandDto createBrandRequest);
        Task<bool> UpdateBrandAsync(Guid id, BrandDto updateBrandRequest);
        Task<bool> DeleteBrandAsync(Guid id);
    }
}
