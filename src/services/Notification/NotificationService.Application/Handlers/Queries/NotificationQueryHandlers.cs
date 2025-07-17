using MediatR;
using NotificationService.Application.Queries;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Application.Handlers.Queries;

public class GetNotificationHistoryQueryHandler : IRequestHandler<GetNotificationHistoryQuery, IEnumerable<NotificationLog>>
{
    private readonly INotificationLogRepository _repository;

    public GetNotificationHistoryQueryHandler(INotificationLogRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<NotificationLog>> Handle(GetNotificationHistoryQuery request, CancellationToken cancellationToken)
    {
        var logs = await _repository.GetByUserIdAsync(request.UserId, request.Page, request.PageSize);
        
        // Apply additional filters if needed
        if (request.Type.HasValue)
        {
            logs = logs.Where(l => l.Type == request.Type.Value);
        }

        if (request.Status.HasValue)
        {
            logs = logs.Where(l => l.Status == request.Status.Value);
        }

        if (request.FromDate.HasValue)
        {
            logs = logs.Where(l => l.SentAt >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            logs = logs.Where(l => l.SentAt <= request.ToDate.Value);
        }

        return logs;
    }
}

public class GetTemplateQueryHandler : IRequestHandler<GetTemplateQuery, NotificationTemplate?>
{
    private readonly INotificationTemplateRepository _repository;

    public GetTemplateQueryHandler(INotificationTemplateRepository repository)
    {
        _repository = repository;
    }

    public async Task<NotificationTemplate?> Handle(GetTemplateQuery request, CancellationToken cancellationToken)
    {
        if (request.Id.HasValue)
        {
            return await _repository.GetByIdAsync(request.Id.Value);
        }

        if (!string.IsNullOrEmpty(request.Name))
        {
            return await _repository.GetByNameAsync(request.Name);
        }

        return null;
    }
}

public class GetAllTemplatesQueryHandler : IRequestHandler<GetAllTemplatesQuery, IEnumerable<NotificationTemplate>>
{
    private readonly INotificationTemplateRepository _repository;

    public GetAllTemplatesQueryHandler(INotificationTemplateRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<NotificationTemplate>> Handle(GetAllTemplatesQuery request, CancellationToken cancellationToken)
    {
        var templates = await _repository.GetAllAsync();

        if (request.Type.HasValue)
        {
            templates = templates.Where(t => t.Type == request.Type.Value);
        }

        if (request.IsActive.HasValue)
        {
            templates = templates.Where(t => t.IsActive == request.IsActive.Value);
        }

        if (!string.IsNullOrEmpty(request.Language))
        {
            templates = templates.Where(t => t.Language == request.Language);
        }

        return templates;
    }
}

public class GetNotificationLogQueryHandler : IRequestHandler<GetNotificationLogQuery, NotificationLog?>
{
    private readonly INotificationLogRepository _repository;

    public GetNotificationLogQueryHandler(INotificationLogRepository repository)
    {
        _repository = repository;
    }

    public async Task<NotificationLog?> Handle(GetNotificationLogQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.Id);
    }
}
