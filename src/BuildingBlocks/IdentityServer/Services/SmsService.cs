namespace IdentityServer.Services;

public interface ISmsService
{
    Task<bool> SendVerificationCodeAsync(string phoneNumber, string code);
    Task<bool> SendWelcomeMessageAsync(string phoneNumber, string userName);
}

public class SmsService : ISmsService
{
    private readonly ILogger<SmsService> _logger;
    private readonly IConfiguration _configuration;

    public SmsService(ILogger<SmsService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<bool> SendVerificationCodeAsync(string phoneNumber, string code)
    {
        try
        {
            // در اینجا می‌توانید از Twilio، کاوه نگار یا سایر سرویس‌های SMS استفاده کنید
            var message = $"کد تایید شما: {code}\nاین کد تا 5 دقیقه معتبر است.";
            
            // برای تست، فقط در لاگ می‌نویسیم
            _logger.LogInformation($"SMS sent to {phoneNumber}: {message}");
            
            // شبیه‌سازی ارسال SMS
            await Task.Delay(100);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ارسال SMS به شماره {PhoneNumber}", phoneNumber);
            return false;
        }
    }

    public async Task<bool> SendWelcomeMessageAsync(string phoneNumber, string userName)
    {
        try
        {
            var message = $"سلام {userName}!\nبه سیستم ما خوش آمدید.";
            
            _logger.LogInformation($"Welcome SMS sent to {phoneNumber}: {message}");
            
            await Task.Delay(100);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ارسال پیام خوش‌آمد به شماره {PhoneNumber}", phoneNumber);
            return false;
        }
    }
}

// سرویس کاوه نگار (اختیاری)
public class KaveNegarSmsService : ISmsService
{
    private readonly ILogger<KaveNegarSmsService> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _apiKey;

    public KaveNegarSmsService(ILogger<KaveNegarSmsService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _apiKey = _configuration["SMS:KaveNegar:ApiKey"] ?? "";
    }

    public async Task<bool> SendVerificationCodeAsync(string phoneNumber, string code)
    {
        try
        {
            // پیاده‌سازی کاوه نگار
            // var api = new Kavenegar.KavenegarApi(_apiKey);
            // var result = await api.Send("1000596446", phoneNumber, code);
            
            _logger.LogInformation($"KaveNegar SMS sent to {phoneNumber} with code: {code}");
            await Task.Delay(100);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ارسال SMS کاوه نگار");
            return false;
        }
    }

    public async Task<bool> SendWelcomeMessageAsync(string phoneNumber, string userName)
    {
        try
        {
            _logger.LogInformation($"KaveNegar welcome SMS sent to {phoneNumber}");
            await Task.Delay(100);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ارسال پیام خوش‌آمد کاوه نگار");
            return false;
        }
    }
}
