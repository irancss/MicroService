using MediatR;
using ProductService.Application.DTOs.Brand;

namespace ProductService.Application.CQRS.Brand.Queries
{
    public class GetBrandByIdQuery : IRequest<BrandDto>
    {
        public string Id { get; set; }
    }
    public class GetBrandByIdQueryHandler : IRequestHandler<GetBrandByIdQuery , BrandDto>
    {
        public async Task<BrandDto> Handle(GetBrandByIdQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
