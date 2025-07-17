using AutoMapper;
using DiscountService.Application.DTOs;
using DiscountService.Domain.Entities;

namespace DiscountService.Application.Mappings;

/// <summary>
/// AutoMapper profile for discount mappings
/// </summary>
public class DiscountProfile : Profile
{
    public DiscountProfile()
    {
        CreateMap<Discount, DiscountDto>().ReverseMap();
        
        CreateMap<DiscountUsageHistory, DiscountUsageHistoryDto>()
            .ForMember(dest => dest.DiscountName, opt => opt.MapFrom(src => src.Discount.Name));

        CreateMap<CreateDiscountRequest, Discount>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CurrentTotalUsage, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UsageHistory, opt => opt.Ignore());

        CreateMap<UpdateDiscountRequest, Discount>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CurrentTotalUsage, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UsageHistory, opt => opt.Ignore());
    }
}
