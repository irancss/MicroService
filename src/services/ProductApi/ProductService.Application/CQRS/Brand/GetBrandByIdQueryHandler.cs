using ProductService.Domain.Interfaces;
using AutoMapper;
using BuildingBlocks.Application.CQRS.Queries;
using ProductService.Application.DTOs.Brand;

namespace ProductService.Application.CQRS.Brand
{
    public class GetBrandByIdQueryHandler : IQueryHandler<GetBrandByIdQuery, BrandDto?>
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;

        public GetBrandByIdQueryHandler(IBrandRepository brandRepository, IMapper mapper)
        {
            _brandRepository = brandRepository;
            _mapper = mapper;
        }

        public async Task<BrandDto?> Handle(GetBrandByIdQuery request, CancellationToken cancellationToken)
        {
            var brand = await _brandRepository.GetByIdAsync(request.BrandId);

            return _mapper.Map<BrandDto?>(brand);
        }
    }
}
