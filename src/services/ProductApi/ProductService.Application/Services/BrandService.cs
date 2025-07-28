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
            throw new NotImplementedException();
        }

        public async Task<BrandDto> GetBrandByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<string> CreateBrandAsync(BrandDto createBrandRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateBrandAsync(Guid id, BrandDto updateBrandRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteBrandAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}

