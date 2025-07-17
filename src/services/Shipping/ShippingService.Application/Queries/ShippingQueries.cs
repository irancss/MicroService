using Shared.Kernel.CQRS;
using ShippingService.Application.DTOs;

namespace ShippingService.Application.Queries;

// Shipping Method Queries
public record GetShippingMethodByIdQuery(Guid Id) : IQuery<ShippingMethodDto?>;

public record GetAllShippingMethodsQuery : IQuery<IEnumerable<ShippingMethodDto>>;

// Main Query for Customer
public record GetAvailableShippingOptionsQuery(
    List<CartItemDto> CartItems,
    CustomerAddressDto CustomerAddress,
    DateTime? PreferredDeliveryDate = null
) : IQuery<IEnumerable<AvailableShippingOptionDto>>;

// Time Slot Queries
public record GetAvailableTimeSlotsQuery(
    Guid ShippingMethodId,
    DateOnly StartDate,
    DateOnly EndDate
) : IQuery<IEnumerable<AvailableTimeSlotDto>>;

public record GetTimeSlotTemplatesByMethodQuery(Guid ShippingMethodId) : IQuery<IEnumerable<TimeSlotTemplateDto>>;
