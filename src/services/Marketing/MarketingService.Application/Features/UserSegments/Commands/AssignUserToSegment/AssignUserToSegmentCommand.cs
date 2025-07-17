using MarketingService.Application.Common;

namespace MarketingService.Application.Features.UserSegments.Commands.AssignUserToSegment;

public record AssignUserToSegmentCommand(
    Guid UserId,
    Guid SegmentId,
    string AssignedBy,
    DateTime? ExpiresAt = null) : ICommand<bool>;
