using MediatR;
using ReportingService.Domain.Events;

namespace ReportingService.Application.Commands.ProcessOrderData;

/// <summary>
/// Command to process OrderCompletedEvent and transform data for analytical database
/// This implements the "T" and "L" parts of ETL (Transform and Load)
/// </summary>
public record ProcessOrderCompletedEventCommand : IRequest<ProcessOrderCompletedEventResponse>
{
    public OrderCompletedEvent OrderEvent { get; init; } = default!;
}

public record ProcessOrderCompletedEventResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public Guid? OrderFactId { get; init; }
}
