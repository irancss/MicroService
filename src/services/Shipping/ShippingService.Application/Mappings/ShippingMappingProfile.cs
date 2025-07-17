using AutoMapper;
using ShippingService.Application.DTOs;
using ShippingService.Domain.Entities;
using ShippingService.Domain.ValueObjects;

namespace ShippingService.Application.Mappings;

public class ShippingMappingProfile : Profile
{
    public ShippingMappingProfile()
    {
        // Entity to DTO mappings
        CreateMap<ShippingMethod, ShippingMethodDto>();
        CreateMap<TimeSlotTemplate, TimeSlotTemplateDto>()
            .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => (int)src.DayOfWeek));
        CreateMap<TimeSlotBooking, TimeSlotBookingDto>();
        CreateMap<CostRule, CostRuleDto>()
            .ForMember(dest => dest.RuleType, opt => opt.MapFrom(src => (int)src.RuleType));
        CreateMap<RestrictionRule, RestrictionRuleDto>()
            .ForMember(dest => dest.RuleType, opt => opt.MapFrom(src => (int)src.RuleType));

        // DTO to Entity mappings (for create operations)
        CreateMap<CreateShippingMethodDto, ShippingMethod>()
            .ConstructUsing(src => new ShippingMethod(src.Name, src.Description, src.BaseCost, src.RequiresTimeSlot));

        // CartItemDto to Domain CartItem
        CreateMap<CartItemDto, CartItem>();
    }
}
