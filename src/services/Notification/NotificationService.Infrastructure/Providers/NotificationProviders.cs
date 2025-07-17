using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace NotificationService.Infrastructure.Providers;

public class SendGridEmailProvider : IEmailProvider
{
    private readonly ISendGridClient _client;
    private readonly SendGridOptions _options;
    private readonly ILogger<SendGridEmailProvider> _logger;

    public SendGridEmailProvider(
        ISendGridClient client,
        IOptions<SendGridOptions> options,
        ILogger<SendGridEmailProvider> logger)
    {
        _client = client;
        _options = options.Value;
        _logger = logger;
    }

    public NotificationType SupportedType => NotificationType.Email;

    public async Task<bool> SendAsync(string recipient, string subject, string body, Dictionary<string, object>? metadata = null)
    {
        return await SendEmailAsync(recipient, subject, body);
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body, string? fromEmail = null, string? fromName = null)
    {
        try
        {
            var from = new EmailAddress(fromEmail ?? _options.FromEmail, fromName ?? _options.FromName);
            var toAddress = new EmailAddress(to);
            
            var msg = MailHelper.CreateSingleEmail(from, toAddress, subject, body, body);
            
            var response = await _client.SendEmailAsync(msg);
            
            var success = response.IsSuccessStatusCode;
            
            if (!success)
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                _logger.LogError("SendGrid failed to send email. Status: {StatusCode}, Response: {Response}",
                    response.StatusCode, responseBody);
            }
            
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email through SendGrid to {Recipient}", to);
            return false;
        }
    }

    public bool IsHealthy()
    {
        // Simple health check - could be enhanced
        return !string.IsNullOrEmpty(_options.ApiKey);
    }
}

public class SendGridOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
}

// Kavenegar SMS Provider (Iranian SMS service)
public class KavenegarSmsProvider : ISmsProvider
{
    private readonly HttpClient _httpClient;
    private readonly KavenegarOptions _options;
    private readonly ILogger<KavenegarSmsProvider> _logger;

    public KavenegarSmsProvider(
        HttpClient httpClient,
        IOptions<KavenegarOptions> options,
        ILogger<KavenegarSmsProvider> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public NotificationType SupportedType => NotificationType.Sms;

    public async Task<bool> SendAsync(string recipient, string subject, string body, Dictionary<string, object>? metadata = null)
    {
        return await SendSmsAsync(recipient, body);
    }

    public async Task<bool> SendSmsAsync(string phoneNumber, string message)
    {
        try
        {
            var url = $"https://api.kavenegar.com/v1/{_options.ApiKey}/sms/send.json";
            
            var parameters = new List<KeyValuePair<string, string>>
            {
                new("receptor", phoneNumber),
                new("message", message),
                new("sender", _options.SenderNumber)
            };

            var content = new FormUrlEncodedContent(parameters);
            var response = await _httpClient.PostAsync(url, content);

            var success = response.IsSuccessStatusCode;
            
            if (!success)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("Kavenegar failed to send SMS. Status: {StatusCode}, Response: {Response}",
                    response.StatusCode, responseBody);
            }
            
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending SMS through Kavenegar to {PhoneNumber}", phoneNumber);
            return false;
        }
    }

    public bool IsHealthy()
    {
        return !string.IsNullOrEmpty(_options.ApiKey) && !string.IsNullOrEmpty(_options.SenderNumber);
    }
}

public class KavenegarOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string SenderNumber { get; set; } = string.Empty;
}
