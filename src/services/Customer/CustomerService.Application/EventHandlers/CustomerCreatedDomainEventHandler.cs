using System.Text.Json;
using CustomerService.Application.Contracts;
using CustomerService.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using BuildingBlocks.Application.CQRS.DomainEvents;
using BuildingBlocks.Infrastructure.Data;

namespace CustomerService.Application.EventHandlers
{
    public class CustomerCreatedDomainEventHandler : INotificationHandler<DomainNotificationBase<CustomerCreatedDomainEvent>>
    {
        private readonly IApplicationDbContext _dbContext; // از DbContext برای افزودن به Outbox استفاده می‌کنیم
        private readonly ILogger<CustomerCreatedDomainEventHandler> _logger;

        public CustomerCreatedDomainEventHandler(IApplicationDbContext dbContext, ILogger<CustomerCreatedDomainEventHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public Task Handle(DomainNotificationBase<CustomerCreatedDomainEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            // رویداد یکپارچه‌سازی را می‌سازیم
            var integrationEvent = new CustomerRegisteredIntegrationEvent
            {
                CustomerId = domainEvent.CustomerId,
                FullName = domainEvent.FullName,
                Email = domainEvent.Email
            };

            // آن را به عنوان یک پیام Outbox برای ارسال در آینده، به DbContext اضافه می‌کنیم
            _dbContext.OutboxMessages.Add(new OutboxMessage
            {
                Id = Guid.NewGuid(),
                OccurredOnUtc = DateTime.UtcNow,
                Type = integrationEvent.GetType().AssemblyQualifiedName!,
                Content = JsonSerializer.Serialize(integrationEvent)
            });

            _logger.LogInformation("CustomerRegisteredIntegrationEvent for customer {CustomerId} was added to the outbox.", domainEvent.CustomerId);

            // نیازی به SaveChanges در اینجا نیست. UnitOfWork این کار را در پایان تراکنش انجام می‌دهد.
            return Task.CompletedTask;
        }
    }
}