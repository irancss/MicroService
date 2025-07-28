using System.ComponentModel.DataAnnotations;
using BuildingBlocks.Abstractions;
using BuildingBlocks.Application.CQRS.Commands;
using CustomerService.Domain.Entities;
using CustomerService.Domain.Exceptions;
using CustomerService.Domain.Interfaces;
using CustomerService.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace CustomerService.Application.Commands.CreateCustomer
{

    public record CreateCustomerCommand(
        string FirstName,
        string LastName,
        string Email,
        string? PhoneNumber,
        string Street,
        string City,
        string State,
        string Country,
        string ZipCode) :  CommandBase<Guid>;

    public class CreateCustomerCommandHandler : ICommandHandler<CreateCustomerCommand, Guid>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork; // UnitOfWork برای مدیریت تراکنش است
        private readonly ILogger<CreateCustomerCommandHandler> _logger;

        public CreateCustomerCommandHandler(
            ICustomerRepository customerRepository,
            IUnitOfWork unitOfWork,
            ILogger<CreateCustomerCommandHandler> logger)
        {
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var existingCustomer = await _customerRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingCustomer != null)
            {
                // از یک Exception سفارشی و معنادار استفاده می‌کنیم
                throw new DuplicateEmailException(request.Email);
            }

            var customer = Customer.Create(request.FirstName, request.LastName, request.Email, request.PhoneNumber);
            var address = new Address(request.Street, request.City, request.State, request.Country, request.ZipCode, true);
            customer.AddAddress(address);

            await _customerRepository.InsertAsync(customer, cancellationToken);

            // نکته کلیدی: ما اینجا فقط تغییرات را برای ذخیره شدن آماده می‌کنیم.
            // TransactionBehavior و MediatorExtensions بقیه کارها را انجام می‌دهند.
            // رویداد دامنه به صورت خودکار قبل از این مرحله، منتشر و هندل خواهد شد.
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Customer with ID {CustomerId} was successfully created.", customer.Id);

            return customer.Id;
        }
    }
    }