using IdentityServer.Models;
using IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using IdentityServer.TempModels;
using IdentityServer8.Stores;

namespace IdentityServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ISmsService _smsService;
    private readonly ILogger<AuthController> _logger;
    private readonly IPersistedGrantStore _persistedGrantStore;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ISmsService smsService, ILogger<AuthController> logger, IPersistedGrantStore persistedGrantStore)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _smsService = smsService;
        _logger = logger;
        _persistedGrantStore = persistedGrantStore;
    }

    [HttpPost("send-verification-code")]
    public async Task<IActionResult> SendVerificationCode([FromBody] SendCodeRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {

            if (!IsValidIranianMobile(request.PhoneNumber))
                return BadRequest(new { message = "فرمت شماره موبایل صحیح نیست" });

            var verificationCode = GenerateVerificationCode();
            var expiry = DateTime.UtcNow.AddMinutes(5);

            var user = await _userManager.FindByNameAsync(request.PhoneNumber);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = request.PhoneNumber,
                    PhoneNumber = request.PhoneNumber,
                    MobileVerificationCode = verificationCode,
                    MobileVerificationCodeExpiry = expiry,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    LockoutEnabled = true // <--- مهم: قفل شدن را برای کاربر جدید فعال کنید
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                    return BadRequest(new { message = "خطا در ایجاد کاربر", errors = result.Errors });
            }
            else
            {
                // اگر کاربر از قبل وجود دارد، کد تایید را آپدیت می‌کنیم
                user.MobileVerificationCode = verificationCode;
                user.MobileVerificationCodeExpiry = expiry;

                // نیازی به ریست کردن شمارنده دستی نیست
                await _userManager.UpdateAsync(user);
            }

            // ... (بقیه کد برای ارسال SMS)
            var smsSent = await _smsService.SendVerificationCodeAsync(request.PhoneNumber, verificationCode);
            if (!smsSent)
                return StatusCode(500, new { message = "خطا در ارسال پیامک" });

            _logger.LogInformation("کد تایید برای شماره {PhoneNumber} ارسال شد", request.PhoneNumber);

            return Ok(new
            {
                message = "کد تایید ارسال شد",
                expiresIn = 300,
                maskedPhone = MaskPhoneNumber(request.PhoneNumber)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ارسال کد تایید");
            return StatusCode(500, new { message = "خطای داخلی سرور" });
        }
    }



    [HttpPost("forgot-password/send-code")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SendPasswordResetCode([FromBody] ForgotPasswordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // 1. ابتدا بررسی می‌کنیم که کاربر حتماً وجود داشته باشد
        var user = await _userManager.FindByNameAsync(request.PhoneNumber);
        if (user == null)
        {
            // به دلایل امنیتی، بهتر است نگوییم کاربر یافت نشد.
            // فقط یک پیام عمومی می‌دهیم که اگر شماره درست باشد، پیامک ارسال می‌شود.
            return Ok(new { message = "در صورتی که شماره موبایل صحیح باشد، کد بازیابی برای شما ارسال خواهد شد." });
        }

        // 2. یک توکن برای بازیابی رمز عبور ایجاد می‌کنیم. این توکن متفاوت از کد SMS است.
        // این توکن را بعداً برای تایید نهایی نیاز داریم.
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        // ما می‌توانیم کد کوتاه‌تر SMS را به عنوان کد تایید استفاده کنیم و توکن اصلی را در دیتابیس ذخیره کنیم.
        // یا ساده‌تر، می‌توانیم از همان منطق کد تایید فعلی استفاده کنیم. بیایید راه ساده‌تر را برویم.

        var verificationCode = GenerateVerificationCode();
        var expiry = DateTime.UtcNow.AddMinutes(10); // 10 دقیقه برای بازیابی

        user.MobileVerificationCode = verificationCode;
        user.MobileVerificationCodeExpiry = expiry;
        await _userManager.UpdateAsync(user);

        // 3. ارسال کد با SMS
        var smsSent = await _smsService.SendVerificationCodeAsync(request.PhoneNumber, verificationCode);
        if (!smsSent)
        {
            return StatusCode(500, new { message = "خطا در ارسال پیامک بازیابی" });
        }

        _logger.LogInformation("کد بازیابی رمز برای {PhoneNumber} ارسال شد", request.PhoneNumber);

        return Ok(new { message = "کد بازیابی با موفقیت ارسال شد." });
    }

    [HttpPost("forgot-password/reset")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPasswordWithCode([FromBody] ResetPasswordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByNameAsync(request.PhoneNumber);
        if (user == null)
        {
            // دوباره پیام عمومی برای امنیت
            return BadRequest(new { message = "اطلاعات نامعتبر است." });
        }

        // 1. بررسی می‌کنیم که آیا کاربر اصلاً کد تاییدی درخواست کرده و کد منقضی نشده باشد
        if (string.IsNullOrEmpty(user.MobileVerificationCode) || user.MobileVerificationCodeExpiry < DateTime.UtcNow)
        {
            return BadRequest(new { message = "کد تایید نامعتبر یا منقضی شده است. لطفاً مجدداً درخواست دهید." });
        }

        // 2. کد تایید وارد شده را با کد ذخیره شده مقایسه می‌کنیم
        if (user.MobileVerificationCode != request.Code)
        {
            return BadRequest(new { message = "کد تایید اشتباه است." });
        }

        // اگر تا اینجا همه چیز درست بود، حالا از توکن داخلی Identity برای ریست کردن رمز استفاده می‌کنیم.
        // این یک لایه امنیتی اضافی است.
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        // 3. رمز عبور را با استفاده از توکن ریست می‌کنیم
        var result = await _userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);

        if (result.Succeeded)
        {
            // 4. پس از موفقیت، کد تایید را از دیتابیس پاک می‌کنیم تا دیگر قابل استفاده نباشد
            user.MobileVerificationCode = null;
            user.MobileVerificationCodeExpiry = null;
            await _userManager.UpdateAsync(user);

            // همچنین تمام session های فعال کاربر را لغو می‌کنیم
            await _persistedGrantStore.RemoveAllAsync(new PersistedGrantFilter { SubjectId = user.Id });

            _logger.LogInformation("رمز عبور کاربر {PhoneNumber} با موفقیت بازیابی شد", request.PhoneNumber);
            return Ok(new { message = "رمز عبور شما با موفقیت تغییر کرد." });
        }

        // اگر خطایی در ResetPasswordAsync رخ داد (مثلاً رمز جدید ضعیف بود)
        var errors = result.Errors.Select(e => e.Description);
        return BadRequest(new { message = "خطا در تغییر رمز عبور", errors });
    }

    [HttpPost("verify-code")]
    public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var user = await _userManager.FindByNameAsync(request.PhoneNumber);
            if (user == null)
                return BadRequest(new { message = "کاربر یافت نشد" });

            // 1. بررسی قفل بودن حساب با مکانیزم Identity
            if (await _userManager.IsLockedOutAsync(user))
            {
                var remainingTime = (user.LockoutEnd.Value - DateTimeOffset.UtcNow).TotalMinutes;
                return BadRequest(new
                {
                    message = $"حساب شما قفل شده است. لطفاً پس از {Math.Ceiling(remainingTime)} دقیقه مجدداً تلاش کنید."
                });
            }

            // 2. بررسی انقضای کد (این منطق سفارشی شماست و باید بماند)
            if (user.MobileVerificationCodeExpiry < DateTime.UtcNow)
            {
                return BadRequest(new { message = "کد تایید منقضی شده است" });
            }

            // 3. بررسی صحت کد
            if (user.MobileVerificationCode != request.Code)
            {
                // افزایش شمارنده تلاش ناموفق با متد داخلی Identity
                await _userManager.AccessFailedAsync(user);

                // بررسی مجدد برای نمایش پیام قفل شدن فوری
                if (await _userManager.IsLockedOutAsync(user))
                {
                    return BadRequest(new { message = "حساب شما به دلیل تلاش‌های ناموفق متعدد برای دقایقی قفل شد." });
                }

                // محاسبه تلاش‌های باقی‌مانده
                var maxAttempts = _userManager.Options.Lockout.MaxFailedAccessAttempts;
                var remainingAttempts = maxAttempts - await _userManager.GetAccessFailedCountAsync(user);

                return BadRequest(new
                {
                    message = "کد تایید اشتباه است",
                    remainingAttempts = remainingAttempts > 0 ? remainingAttempts : 0
                });
            }

            // 4. تایید موفق
            // ریست کردن شمارنده تلاش ناموفق
            await _userManager.ResetAccessFailedCountAsync(user);

            user.IsMobileVerified = true;
            user.PhoneNumberConfirmed = true;
            user.MobileVerificationCode = null; // پاک کردن کد پس از استفاده
            user.MobileVerificationCodeExpiry = null;
            user.LastLoginDate = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            // ورود کاربر
            await _signInManager.SignInAsync(user, isPersistent: false);

            _logger.LogInformation("کاربر {PhoneNumber} با موفقیت وارد شد", request.PhoneNumber);

            return Ok(new
            {
                message = "ورود موفقیت‌آمیز",
                user = new
                {
                    id = user.Id,
                    phoneNumber = user.PhoneNumber,
                    fullName = user.FullName,
                    isActive = user.IsActive
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در تایید کد");
            return StatusCode(500, new { message = "خطای داخلی سرور" });
        }

    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok(new { message = "خروج موفقیت‌آمیز" });
    }

    private static string GenerateVerificationCode()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }

    private static bool IsValidIranianMobile(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
            return false;

        // حذف کاراکترهای غیرضروری
        phoneNumber = phoneNumber.Replace("+98", "").Replace(" ", "").Replace("-", "");

        // بررسی طول و شروع با 09
        return phoneNumber.Length == 11 && phoneNumber.StartsWith("09");
    }

    private static string MaskPhoneNumber(string phoneNumber)
    {
        if (phoneNumber.Length >= 7)
        {
            return phoneNumber.Substring(0, 4) + "***" + phoneNumber.Substring(phoneNumber.Length - 4);
        }
        return phoneNumber;
    }


}

public class SendCodeRequest
{
    [Required(ErrorMessage = "شماره موبایل الزامی است")]
    [Phone(ErrorMessage = "فرمت شماره موبایل صحیح نیست")]
    public string PhoneNumber { get; set; } = "";

    [MaxLength(50)]
    public string? FirstName { get; set; }

    [MaxLength(50)]
    public string? LastName { get; set; }
}

public class VerifyCodeRequest
{
    [Required(ErrorMessage = "شماره موبایل الزامی است")]
    public string PhoneNumber { get; set; } = "";

    [Required(ErrorMessage = "کد تایید الزامی است")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "کد تایید باید 6 رقم باشد")]
    public string Code { get; set; } = "";
}
