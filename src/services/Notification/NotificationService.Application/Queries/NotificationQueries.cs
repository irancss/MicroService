using MediatR;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enums;

namespace NotificationService.Application.Queries;

public record GetNotificationHistoryQuery : IRequest<IEnumerable<NotificationLog>>
{
    public string UserId { get; init; } = string.Empty;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public NotificationType? Type { get; init; }
    public NotificationStatus? Status { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}

public record GetTemplateQuery : IRequest<NotificationTemplate?>
{
    public Guid? Id { get; init; }
    public string? Name { get; init; }
}

public record GetAllTemplatesQuery : IRequest<IEnumerable<NotificationTemplate>>
{
    public NotificationType? Type { get; init; }
    public bool? IsActive { get; init; }
    public string? Language { get; init; }
}

public record GetNotificationLogQuery : IRequest<NotificationLog?>
{
    public Guid Id { get; init; }
}
