using MediatR;
using NotificationService.Application.Commands;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Application.Handlers.Commands;

public class QueueNotificationFromEventCommandHandler : IRequestHandler<QueueNotificationFromEventCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly INotificationTemplateRepository _templateRepository;
    private readonly ITemplateEngine _templateEngine;

    public QueueNotificationFromEventCommandHandler(
        IMediator mediator,
        INotificationTemplateRepository templateRepository,
        ITemplateEngine templateEngine)
    {
        _mediator = mediator;
        _templateRepository = templateRepository;
        _templateEngine = templateEngine;
    }

    public async Task<bool> Handle(QueueNotificationFromEventCommand request, CancellationToken cancellationToken)
    {
        // Get template based on event type
        var template = await GetTemplateForEvent(request.EventType);
        if (template == null)
        {
            // Log that no template found for this event type
            return false;
        }

        // Process template with payload data
        var processedSubject = _templateEngine.ProcessTemplate(template.Subject, request.Payload);
        var processedBody = _templateEngine.ProcessTemplate(template.Body, request.Payload);

        // Extract recipient from payload
        var recipient = ExtractRecipient(request.Payload, template.Type);
        if (string.IsNullOrEmpty(recipient))
        {
            return false;
        }

        // Send specific command based on notification type
        return template.Type switch
        {
            NotificationType.Email => await _mediator.Send(new SendEmailCommand
            {
                To = recipient,
                Subject = processedSubject,
                Body = processedBody,
                UserId = request.UserId,
                Metadata = request.Payload,
                Priority = request.Priority,
                TemplateId = template.Id
            }, cancellationToken),

            NotificationType.Sms => await _mediator.Send(new SendSmsCommand
            {
                PhoneNumber = recipient,
                Message = processedBody,
                UserId = request.UserId,
                Metadata = request.Payload,
                Priority = request.Priority,
                TemplateId = template.Id
            }, cancellationToken),

            _ => false
        };
    }

    private async Task<NotificationTemplate?> GetTemplateForEvent(string eventType)
    {
        // This could be enhanced with more sophisticated template selection logic
        var templateName = $"{eventType}_notification";
        return await _templateRepository.GetByNameAsync(templateName);
    }

    private static string ExtractRecipient(Dictionary<string, object> payload, NotificationType type)
    {
        return type switch
        {
            NotificationType.Email => payload.TryGetValue("email", out var email) ? email?.ToString() ?? string.Empty : string.Empty,
            NotificationType.Sms => payload.TryGetValue("phoneNumber", out var phone) ? phone?.ToString() ?? string.Empty : string.Empty,
            _ => string.Empty
        };
    }
}

public class CreateTemplateCommandHandler : IRequestHandler<CreateTemplateCommand, Guid>
{
    private readonly INotificationTemplateRepository _repository;

    public CreateTemplateCommandHandler(INotificationTemplateRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = new NotificationTemplate
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Type = request.Type,
            Subject = request.Subject,
            Body = request.Body,
            Language = request.Language,
            Parameters = request.Parameters,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = request.CreatedBy,
            UpdatedBy = request.CreatedBy
        };

        await _repository.CreateAsync(template);
        return template.Id;
    }
}

public class UpdateTemplateCommandHandler : IRequestHandler<UpdateTemplateCommand, bool>
{
    private readonly INotificationTemplateRepository _repository;

    public UpdateTemplateCommandHandler(INotificationTemplateRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(UpdateTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = await _repository.GetByIdAsync(request.Id);
        if (template == null)
            return false;

        template.Name = request.Name;
        template.Type = request.Type;
        template.Subject = request.Subject;
        template.Body = request.Body;
        template.Language = request.Language;
        template.Parameters = request.Parameters;
        template.IsActive = request.IsActive;
        template.UpdatedAt = DateTime.UtcNow;
        template.UpdatedBy = request.UpdatedBy;

        await _repository.UpdateAsync(template);
        return true;
    }
}
