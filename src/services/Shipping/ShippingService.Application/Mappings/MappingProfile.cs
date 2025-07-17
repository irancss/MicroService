using AutoMapper;
using ShippingService.Application.DTOs;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Enums;

namespace ShippingService.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ShippingMethod, ShippingMethodDto>();
        
        // Mapping for PremiumSubscription
        CreateMap<PremiumSubscription, PremiumSubscriptionDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<SubscriptionUsageLog, SubscriptionUsageLogDto>();

        // Mapping for FreeShippingRule
        CreateMap<FreeShippingRule, FreeShippingRuleDto>()
            .ForMember(dest => dest.DiscountType, opt => opt.MapFrom(src => src.DiscountType.ToString()));
            
        CreateMap<FreeShippingCondition, FreeShippingConditionDto>()
            .ForMember(dest => dest.ConditionType, opt => opt.MapFrom(src => src.ConditionType.ToString()))
            .ForMember(dest => dest.Operator, opt => opt.MapFrom(src => src.Operator.ToString()))
            .ForMember(dest => dest.ValueType, opt => opt.MapFrom(src => src.ValueType.ToString()));
    }
}