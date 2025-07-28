using CustomerService.Application.Dtos;
using AutoMapper;
using CustomerService.Domain.Entities;
using CustomerService.Domain.ValueObjects;

namespace CustomerService.Application.Mappings
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CustomerDto>();
            CreateMap<Address, AddressDto>();
        }
    }
}
