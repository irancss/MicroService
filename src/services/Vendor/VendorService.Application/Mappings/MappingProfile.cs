using AutoMapper;
using VendorService.Domain.Models;

namespace VendorService.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Vendor, VendorDto>()
            .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Contact.PhoneNumber))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Contact.Email));
    }
}