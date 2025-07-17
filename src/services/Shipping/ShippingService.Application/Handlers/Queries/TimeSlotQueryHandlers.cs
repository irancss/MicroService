using AutoMapper;
using Shared.Kernel.CQRS;
using ShippingService.Application.DTOs;
using ShippingService.Application.Queries;
using ShippingService.Domain.Repositories;

namespace ShippingService.Application.Handlers.Queries;

public class GetTimeSlotTemplatesByMethodQueryHandler : IQueryHandler<GetTimeSlotTemplatesByMethodQuery, IEnumerable<TimeSlotTemplateDto>>
{
    private readonly ITimeSlotRepository _repository;
    private readonly IMapper _mapper;

    public GetTimeSlotTemplatesByMethodQueryHandler(ITimeSlotRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TimeSlotTemplateDto>> Handle(GetTimeSlotTemplatesByMethodQuery request, CancellationToken cancellationToken)
    {
        var templates = await _repository.GetTemplatesByShippingMethodAsync(request.ShippingMethodId, cancellationToken);
        return _mapper.Map<IEnumerable<TimeSlotTemplateDto>>(templates);
    }
}
