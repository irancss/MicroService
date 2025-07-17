using MediatR;
using ShippingService.Domain.Enums;

namespace ShippingService.Application.Queries.Returns;

public record GetReturnByIdQuery(Guid ReturnId) : IRequest<ReturnDto?>;

public record GetReturnByTrackingNumberQuery(string TrackingNumber) : IRequest<ReturnDto?>;

public record GetReturnsByCustomerIdQuery(string CustomerId) : IRequest<IEnumerable<ReturnDto>>;

public record GetPendingReturnsQuery() : IRequest<IEnumerable<ReturnDto>>;

public record TrackReturnQuery(string ReturnTrackingNumber) : IRequest<ReturnTrackingDto?>;

public class ReturnDto
{
    public Guid Id { get; set; }
    public Guid OriginalShipmentId { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public string ReturnTrackingNumber { get; set; } = string.Empty;
    public ReturnReason Reason { get; set; }
    public string? ReasonDescription { get; set; }
    public ReturnStatus Status { get; set; }
    public DateTime RequestedDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public decimal RefundAmount { get; set; }
    public bool IsRefundProcessed { get; set; }
    public DateTime? RefundProcessedDate { get; set; }
    public string? CollectionAddress { get; set; }
    public DateTime? CollectionDate { get; set; }
}

public class ReturnTrackingDto
{
    public Guid ReturnId { get; set; }
    public string ReturnTrackingNumber { get; set; } = string.Empty;
    public ReturnStatus CurrentStatus { get; set; }
    public DateTime RequestedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public List<ReturnTrackingEventDto> TrackingHistory { get; set; } = new();
}

public class ReturnTrackingEventDto
{
    public ReturnStatus Status { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Notes { get; set; }
}
