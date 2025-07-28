using AutoMapper;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.CQRS.Queries;
using CustomerService.Application.Dtos;
using CustomerService.Domain.Exceptions;
using CustomerService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CustomerService.Application.Queries.GetCustomerById
{
    public record GetCustomerByIdQuery(Guid Id) : IQuery<CustomerDto>;

    public class GetCustomerByIdQueryHandler : IQueryHandler<GetCustomerByIdQuery, CustomerDto>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCustomerByIdQueryHandler> _logger;

        public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository, IMapper mapper, ILogger<GetCustomerByIdQueryHandler> logger)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CustomerDto> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching customer with ID: {CustomerId}", request.Id);

            var customer = await _customerRepository.GetByIdAsync(request.Id);

            if (customer == null)
            {
                throw new CustomerNotFoundException(request.Id);
            }

            return _mapper.Map<CustomerDto>(customer);
        }
    }
}
