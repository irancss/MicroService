using MediatR;
using ShippingService.Domain.Enums;

namespace ShippingService.Application.Commands.Returns;

public record CreateReturnRequestCommand(
    Guid OriginalShipmentId,
    string CustomerId,
    ReturnReason Reason,
    string? ReasonDescription = null,
    string? CollectionAddress = null
) : IRequest<CreateReturnRequestResult>;

public record CreateReturnRequestResult(
    Guid ReturnId,
    string ReturnTrackingNumber,
    ReturnStatus Status
);

public record ApproveReturnCommand(
    Guid ReturnId,
    string ApprovedByUserId,
    decimal RefundAmount,
    string? ApprovalNotes = null
) : IRequest;

public record RejectReturnCommand(
    Guid ReturnId,
    string RejectedByUserId,
    string Reason
) : IRequest;

public record StartReturnCollectionCommand(
    Guid ReturnId,
    DateTime CollectionDate,
    string? Notes = null
) : IRequest;

public record CompleteReturnCommand(
    Guid ReturnId
) : IRequest;

public record ProcessReturnRefundCommand(
    Guid ReturnId,
    string TransactionId
) : IRequest;
