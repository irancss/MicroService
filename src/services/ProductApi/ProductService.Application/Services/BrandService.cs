using AutoMapper;
using ProductService.Application.DTOs.Brand;
using ProductService.Application.Interfaces;
using ProductService.Domain.Interfaces;
using Sieve.Models;
using ProductService.Application.DTOs;
using ProductService.Domain.Models;

namespace ProductService.Application.Services
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;

        public BrandService(IBrandRepository brandRepository, IMapper mapper)
        {
            _brandRepository = brandRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedList<BrandListDto>> GetAllBrandsAsync(SieveModel sieveModel)
        {
  
            var brandEntities = await _brandRepository.GetPagedAndSortedAsync(sieveModel);
            var totalCount = brandEntities.Count();
           

            var brandViewModels = brandEntities.Select(brand => new BrandListDto
            {
                Id = brand.Id,
                Name = brand.Name
                // Map other properties as needed
            }).ToList();

            var pageNumber = sieveModel.Page ?? 1;
            // Consider having a default page size defined in configuration
            var pageSize = sieveModel.PageSize ?? 10; 

            // Assuming PaginatedList constructor is:
            // public PaginatedList(IEnumerable<TItem> items, int totalCount, int pageNumber, int pageSize)
            // And PaginatedList<T> where T is BrandListDto
            return new PaginatedList<BrandListDto>(brandViewModels, totalCount, pageNumber, pageSize);
        }

        public async Task<BrandDto> GetBrandByIdAsync(string id)
        {
            var brandEntity = await _brandRepository.GetByIdAsync(id);
            if (brandEntity is null)
            {
                // Or a custom NotFoundException
                throw new KeyNotFoundException($"Brand with ID {id} not found.");
            }

            return new BrandDto
            {
                Id = brandEntity.Id,
                Name = brandEntity.Name
                // Map other properties
            };
        }

        public async Task<string> CreateBrandAsync(BrandDto createBrandRequest)
        {
            var brandEntity = new Brand // Maps DTO to Domain Entity
            {
                Name = createBrandRequest.Name
                // Map other properties from createBrandRequest
            };

            // Assuming AddAsync returns the created entity with its ID populated
            var createdBrand = await _brandRepository.AddAsync(brandEntity);

            // Return the ID of the newly created brand as a string
            return createdBrand.Id.ToString();
        }

        public async Task<bool> UpdateBrandAsync(string id, BrandDto updateBrandRequest)
        {
            var brandEntity = await _brandRepository.GetByIdAsync(id);
            if (brandEntity is null)
            {
                return false; // Or throw KeyNotFoundException
            }

            // Map properties from updateBrandRequest to the existing brandEntity
            brandEntity.Name = updateBrandRequest.Name;
            // Update other properties as needed

            // Assuming UpdateAsync is void and throws on failure, or the repository handles save changes.
            await _brandRepository.UpdateAsync(brandEntity);
            return true; // Assume success if no exception is thrown
        }
        

        public async Task<bool> DeleteBrandAsync(string id)
        {
            var brandEntity = await _brandRepository.GetByIdAsync(id);
            if (brandEntity is null)
            {
                return false; // Or throw KeyNotFoundException
            }

            // Assuming DeleteAsync takes the entity id and is void, or the repository handles save changes.
            await _brandRepository.DeleteAsync(id); 
            return true; // Assume success if no exception is thrown
        }
    }
}

