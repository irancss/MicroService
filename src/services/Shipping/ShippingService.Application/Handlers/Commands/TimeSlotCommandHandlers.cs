using Shared.Kernel.CQRS;
using ShippingService.Application.Commands;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Repositories;
using DayOfWeekEnum = ShippingService.Domain.Enums.DayOfWeek;

namespace ShippingService.Application.Handlers.Commands;

public class CreateTimeSlotTemplateCommandHandler : ICommandHandler<CreateTimeSlotTemplateCommand, Guid>
{
    private readonly ITimeSlotRepository _repository;
    private readonly IShippingMethodRepository _shippingMethodRepository;

    public CreateTimeSlotTemplateCommandHandler(
        ITimeSlotRepository repository,
        IShippingMethodRepository shippingMethodRepository)
    {
        _repository = repository;
        _shippingMethodRepository = shippingMethodRepository;
    }

    public async Task<Guid> Handle(CreateTimeSlotTemplateCommand request, CancellationToken cancellationToken)
    {
        // Verify shipping method exists
        var exists = await _shippingMethodRepository.ExistsAsync(request.ShippingMethodId, cancellationToken);
        if (!exists)
            throw new KeyNotFoundException($"Shipping method with ID {request.ShippingMethodId} not found");

        var dayOfWeek = (DayOfWeekEnum)request.DayOfWeek;
        var template = new TimeSlotTemplate(
            request.ShippingMethodId,
            dayOfWeek,
            request.StartTime,
            request.EndTime,
            request.Capacity);

        await _repository.AddTemplateAsync(template, cancellationToken);
        return template.Id;
    }
}

public class ReserveTimeSlotCommandHandler : ICommandHandler<ReserveTimeSlotCommand, Guid>
{
    private readonly ITimeSlotRepository _repository;

    public ReserveTimeSlotCommandHandler(ITimeSlotRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(ReserveTimeSlotCommand request, CancellationToken cancellationToken)
    {
        // Get all templates for the shipping method and find a matching one
        var templates = await _repository.GetTemplatesByShippingMethodAsync(request.ShippingMethodId, cancellationToken);
        
        var dayOfWeek = (DayOfWeekEnum)((int)request.Date.DayOfWeek);
        var matchingTemplate = templates.FirstOrDefault(t => 
            t.DayOfWeek == dayOfWeek && 
            t.IsActive &&
            request.StartTime >= t.StartTime && 
            request.EndTime <= t.EndTime);

        if (matchingTemplate == null)
            throw new InvalidOperationException("No matching time slot template found for the specified time");

        // Check availability atomically
        var isAvailable = await _repository.IsTimeSlotAvailableAsync(matchingTemplate.Id, request.Date, cancellationToken);
        if (!isAvailable)
            throw new InvalidOperationException("Time slot is not available");

        var booking = new TimeSlotBooking(
            matchingTemplate.Id,
            request.Date,
            request.StartTime,
            request.EndTime,
            request.CustomerId,
            request.OrderId);

        await _repository.AddBookingAsync(booking, cancellationToken);
        return booking.Id;
    }
}
