using Hangfire;
using MediatR;
using MarketingService.Application.Features.UserSegments.Commands.AssignUserToSegment;
using MarketingService.Domain.Interfaces;
using MarketingService.Domain.ValueObjects;

namespace MarketingService.Infrastructure.BackgroundJobs;

public class UserSegmentationJob
{
    private readonly IMediator _mediator;
    private readonly IUserSegmentRepository _segmentRepository;

    public UserSegmentationJob(IMediator mediator, IUserSegmentRepository segmentRepository)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _segmentRepository = segmentRepository ?? throw new ArgumentNullException(nameof(segmentRepository));
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task ProcessUserSegmentation(Guid userId, Dictionary<string, object> userAttributes)
    {
        try
        {
            var activeSegments = await _segmentRepository.GetActiveAsync();

            foreach (var segment in activeSegments)
            {
                if (ShouldUserBeInSegment(userAttributes, segment.Criteria))
                {
                    var command = new AssignUserToSegmentCommand(
                        userId,
                        segment.Id,
                        "system-segmentation-job");

                    await _mediator.Send(command);
                }
            }
        }
        catch (Exception ex)
        {
            // Log the exception
            throw new InvalidOperationException($"Failed to process user segmentation for user {userId}", ex);
        }
    }

    [AutomaticRetry(Attempts = 2)]
    public async Task ProcessBulkUserSegmentation(List<Guid> userIds)
    {
        foreach (var userId in userIds)
        {
            BackgroundJob.Enqueue<UserSegmentationJob>(job => 
                job.ProcessUserSegmentation(userId, GetUserAttributes(userId)));
        }
    }

    private bool ShouldUserBeInSegment(Dictionary<string, object> userAttributes, List<SegmentCriteria> criteria)
    {
        // Simple rule evaluation - can be made more sophisticated
        foreach (var criterion in criteria)
        {
            if (!userAttributes.ContainsKey(criterion.Field))
                return false;

            var userValue = userAttributes[criterion.Field];
            
            switch (criterion.Operator.ToLower())
            {
                case "equals":
                    if (!userValue.ToString()!.Equals(criterion.Value, StringComparison.OrdinalIgnoreCase))
                        return false;
                    break;
                    
                case "greater_than":
                    if (decimal.TryParse(userValue.ToString(), out decimal userDecimal) &&
                        decimal.TryParse(criterion.Value, out decimal criteriaDecimal))
                    {
                        if (userDecimal <= criteriaDecimal)
                            return false;
                    }
                    else
                        return false;
                    break;
                    
                case "less_than":
                    if (decimal.TryParse(userValue.ToString(), out decimal userDecimal2) &&
                        decimal.TryParse(criterion.Value, out decimal criteriaDecimal2))
                    {
                        if (userDecimal2 >= criteriaDecimal2)
                            return false;
                    }
                    else
                        return false;
                    break;
                    
                case "contains":
                    if (!userValue.ToString()!.Contains(criterion.Value, StringComparison.OrdinalIgnoreCase))
                        return false;
                    break;
                    
                default:
                    return false;
            }
        }
        
        return true;
    }

    private Dictionary<string, object> GetUserAttributes(Guid userId)
    {
        // This would typically fetch user data from a user service or database
        // For now, returning a mock implementation
        return new Dictionary<string, object>
        {
            ["Age"] = 25,
            ["Location"] = "Tehran",
            ["PurchaseAmount"] = 1000.0m,
            ["UserType"] = "Premium"
        };
    }
}
