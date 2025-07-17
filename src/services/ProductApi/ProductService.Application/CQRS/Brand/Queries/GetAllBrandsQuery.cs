using AutoMapper;
using MediatR;
using ProductService.Application.DTOs;
using ProductService.Application.DTOs.Brand;
using ProductService.Application.Interfaces;
using Sieve.Models;
using Sieve.Services;

namespace ProductService.Application.CQRS.Brand.Queries
{
    public class GetAllBrandsQuery : IRequest<PaginatedList<BrandListDto>>
    {
        public SieveModel SieveModel { get; set; }
    }

    public class GetAllBrandsQueryHandler : IRequestHandler<GetAllBrandsQuery, PaginatedList<BrandListDto>>
    {
        private readonly IBrandService _brandService;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _processor;

        public GetAllBrandsQueryHandler(IBrandService brandService, IMapper mapper, ISieveProcessor processor)
        {
            _brandService = brandService;
            _mapper = mapper;
            _processor = processor;
        }

        public async Task<PaginatedList<BrandListDto>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
        {
            var brands = await _brandService.GetAllBrandsAsync(request.SieveModel);
            var brandDtos = _mapper.Map<List<BrandListDto>>(brands.Items);
            return new PaginatedList<BrandListDto>(
                brandDtos,
                brands.TotalCount,
                brands.PageNumber,
                brands.TotalPages
            );
        }
    }
}
