using MediatR;
using NotificationService.Application.Commands;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Application.Handlers.Commands;

public class SendEmailCommandHandler : IRequestHandler<SendEmailCommand, bool>
{
    private readonly IEmailProvider _emailProvider;
    private readonly INotificationLogRepository _logRepository;

    public SendEmailCommandHandler(IEmailProvider emailProvider, INotificationLogRepository logRepository)
    {
        _emailProvider = emailProvider;
        _logRepository = logRepository;
    }

    public async Task<bool> Handle(SendEmailCommand request, CancellationToken cancellationToken)
    {
        var log = new NotificationLog
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Type = NotificationType.Email,
            Recipient = request.To,
            Subject = request.Subject,
            Body = request.Body,
            Status = NotificationStatus.Pending,
            SentAt = DateTime.UtcNow,
            Metadata = request.Metadata,
            TemplateId = request.TemplateId
        };

        try
        {
            await _logRepository.CreateAsync(log);

            var success = await _emailProvider.SendEmailAsync(
                request.To, 
                request.Subject, 
                request.Body, 
                request.FromEmail, 
                request.FromName);

            log.Status = success ? NotificationStatus.Sent : NotificationStatus.Failed;
            log.Provider = _emailProvider.GetType().Name;

            if (!success)
            {
                log.ErrorMessage = "Failed to send email through provider";
            }

            await _logRepository.UpdateAsync(log);
            return success;
        }
        catch (Exception ex)
        {
            log.Status = NotificationStatus.Failed;
            log.ErrorMessage = ex.Message;
            await _logRepository.UpdateAsync(log);
            throw;
        }
    }
}

public class SendSmsCommandHandler : IRequestHandler<SendSmsCommand, bool>
{
    private readonly ISmsProvider _smsProvider;
    private readonly INotificationLogRepository _logRepository;

    public SendSmsCommandHandler(ISmsProvider smsProvider, INotificationLogRepository logRepository)
    {
        _smsProvider = smsProvider;
        _logRepository = logRepository;
    }

    public async Task<bool> Handle(SendSmsCommand request, CancellationToken cancellationToken)
    {
        var log = new NotificationLog
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Type = NotificationType.Sms,
            Recipient = request.PhoneNumber,
            Subject = "SMS",
            Body = request.Message,
            Status = NotificationStatus.Pending,
            SentAt = DateTime.UtcNow,
            Metadata = request.Metadata,
            TemplateId = request.TemplateId
        };

        try
        {
            await _logRepository.CreateAsync(log);

            var success = await _smsProvider.SendSmsAsync(request.PhoneNumber, request.Message);

            log.Status = success ? NotificationStatus.Sent : NotificationStatus.Failed;
            log.Provider = _smsProvider.GetType().Name;

            if (!success)
            {
                log.ErrorMessage = "Failed to send SMS through provider";
            }

            await _logRepository.UpdateAsync(log);
            return success;
        }
        catch (Exception ex)
        {
            log.Status = NotificationStatus.Failed;
            log.ErrorMessage = ex.Message;
            await _logRepository.UpdateAsync(log);
            throw;
        }
    }
}
