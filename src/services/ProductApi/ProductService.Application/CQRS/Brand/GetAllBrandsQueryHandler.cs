using ProductService.Domain.Interfaces;
using AutoMapper;
using BuildingBlocks.Application.CQRS.Queries;
using ProductService.Application.DTOs.Brand;

namespace ProductService.Application.CQRS.Brand
{
    public class GetAllBrandsQueryHandler : IQueryHandler<GetAllBrandsQuery, IEnumerable<BrandDto>>
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;

        public GetAllBrandsQueryHandler(IBrandRepository brandRepository, IMapper mapper)
        {
            _brandRepository = brandRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BrandDto>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
        {
            var brands = await _brandRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<BrandDto>>(brands);
        }
    }
}
