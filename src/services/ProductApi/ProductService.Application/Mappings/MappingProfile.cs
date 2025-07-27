using AutoMapper;
using ProductService.Application.DTOs.Product;
using ProductService.Domain.Models;

namespace ProductService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // IMapFrom به صورت خودکار مپ‌ها را پیدا می‌کند، اما برای موارد پیچیده‌تر، اینجا پیکربندی می‌کنیم.
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Value)) // Map از ValueObject
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.Value))
                .ForMember(dest => dest.Sku, opt => opt.MapFrom(src => src.Sku.Value))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : null))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.ProductCategories.Select(pc => pc.Category)));

            CreateMap<Category, CategoryDto>();
            CreateMap<Brand, BrandDto>();
        }
    }
}
