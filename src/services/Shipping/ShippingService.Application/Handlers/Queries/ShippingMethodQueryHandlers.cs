using AutoMapper;
using Shared.Kernel.CQRS;
using ShippingService.Application.DTOs;
using ShippingService.Application.Queries;
using ShippingService.Domain.Repositories;

namespace ShippingService.Application.Handlers.Queries;

public class GetShippingMethodByIdQueryHandler : IQueryHandler<GetShippingMethodByIdQuery, ShippingMethodDto?>
{
    private readonly IShippingMethodRepository _repository;
    private readonly IMapper _mapper;

    public GetShippingMethodByIdQueryHandler(IShippingMethodRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ShippingMethodDto?> Handle(GetShippingMethodByIdQuery request, CancellationToken cancellationToken)
    {
        var shippingMethod = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return shippingMethod != null ? _mapper.Map<ShippingMethodDto>(shippingMethod) : null;
    }
}

public class GetAllShippingMethodsQueryHandler : IQueryHandler<GetAllShippingMethodsQuery, IEnumerable<ShippingMethodDto>>
{
    private readonly IShippingMethodRepository _repository;
    private readonly IMapper _mapper;

    public GetAllShippingMethodsQueryHandler(IShippingMethodRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ShippingMethodDto>> Handle(GetAllShippingMethodsQuery request, CancellationToken cancellationToken)
    {
        var shippingMethods = await _repository.GetAllActiveAsync(cancellationToken);
        return _mapper.Map<IEnumerable<ShippingMethodDto>>(shippingMethods);
    }
}
