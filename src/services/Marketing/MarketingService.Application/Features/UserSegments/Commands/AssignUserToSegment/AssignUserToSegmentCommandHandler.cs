using MarketingService.Application.Common;
using MarketingService.Domain.Entities;
using MarketingService.Domain.Interfaces;

namespace MarketingService.Application.Features.UserSegments.Commands.AssignUserToSegment;

public class AssignUserToSegmentCommandHandler : ICommandHandler<AssignUserToSegmentCommand, bool>
{
    private readonly IUserSegmentRepository _segmentRepository;
    private readonly IUserSegmentMembershipRepository _membershipRepository;

    public AssignUserToSegmentCommandHandler(
        IUserSegmentRepository segmentRepository,
        IUserSegmentMembershipRepository membershipRepository)
    {
        _segmentRepository = segmentRepository ?? throw new ArgumentNullException(nameof(segmentRepository));
        _membershipRepository = membershipRepository ?? throw new ArgumentNullException(nameof(membershipRepository));
    }

    public async Task<bool> Handle(AssignUserToSegmentCommand request, CancellationToken cancellationToken)
    {
        // Verify segment exists and is active
        var segment = await _segmentRepository.GetByIdAsync(request.SegmentId, cancellationToken);
        if (segment == null || !segment.IsActive)
        {
            throw new InvalidOperationException($"Active segment with ID '{request.SegmentId}' not found");
        }

        // Check if user is already assigned to this segment
        var existingMembership = await _membershipRepository.GetByUserAndSegmentAsync(
            request.UserId, 
            request.SegmentId, 
            cancellationToken);

        if (existingMembership != null)
        {
            // If existing membership is inactive, reactivate it
            if (!existingMembership.IsCurrentlyActive)
            {
                existingMembership.Activate();
                if (request.ExpiresAt.HasValue)
                {
                    existingMembership.ExtendExpiry(request.ExpiresAt.Value);
                }
                await _membershipRepository.UpdateAsync(existingMembership, cancellationToken);
                return true;
            }
            
            // Already active membership exists
            return false;
        }

        // Create new membership
        var membership = new UserSegmentMembership(
            request.UserId,
            request.SegmentId,
            request.AssignedBy,
            request.ExpiresAt);

        await _membershipRepository.AddAsync(membership, cancellationToken);
        return true;
    }
}
