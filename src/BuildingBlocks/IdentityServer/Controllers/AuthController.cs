using IdentityServer.Models;
using IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ISmsService _smsService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ISmsService smsService,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _smsService = smsService;
        _logger = logger;
    }

    [HttpPost("send-verification-code")]
    public async Task<IActionResult> SendVerificationCode([FromBody] SendCodeRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // بررسی فرمت شماره موبایل
            if (!IsValidIranianMobile(request.PhoneNumber))
            {
                return BadRequest(new { message = "فرمت شماره موبایل صحیح نیست" });
            }

            // تولید کد 6 رقمی
            var verificationCode = GenerateVerificationCode();
            var expiry = DateTime.UtcNow.AddMinutes(5);

            // پیدا کردن یا ایجاد کاربر
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
                    LastName = request.LastName
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(new { message = "خطا در ایجاد کاربر", errors = result.Errors });
                }
            }
            else
            {
                // بروزرسانی کد تایید
                user.MobileVerificationCode = verificationCode;
                user.MobileVerificationCodeExpiry = expiry;
                user.FailedLoginAttempts = 0;
                
                await _userManager.UpdateAsync(user);
            }

            // ارسال SMS
            var smsSent = await _smsService.SendVerificationCodeAsync(request.PhoneNumber, verificationCode);
            if (!smsSent)
            {
                return StatusCode(500, new { message = "خطا در ارسال پیامک" });
            }

            _logger.LogInformation("کد تایید برای شماره {PhoneNumber} ارسال شد", request.PhoneNumber);

            return Ok(new { 
                message = "کد تایید ارسال شد", 
                expiresIn = 300, // 5 دقیقه
                maskedPhone = MaskPhoneNumber(request.PhoneNumber)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ارسال کد تایید");
            return StatusCode(500, new { message = "خطای داخلی سرور" });
        }
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
            {
                return BadRequest(new { message = "کاربر یافت نشد" });
            }

            // بررسی قفل بودن حساب
            if (user.LockoutEndDate.HasValue && user.LockoutEndDate > DateTime.UtcNow)
            {
                var remainingTime = user.LockoutEndDate.Value - DateTime.UtcNow;
                return BadRequest(new { 
                    message = $"حساب شما قفل شده است. تا {remainingTime.Minutes} دقیقه دیگر تلاش کنید." 
                });
            }

            // بررسی انقضای کد
            if (user.MobileVerificationCodeExpiry < DateTime.UtcNow)
            {
                return BadRequest(new { message = "کد تایید منقضی شده است" });
            }

            // بررسی صحت کد
            if (user.MobileVerificationCode != request.Code)
            {
                user.FailedLoginAttempts++;
                
                // قفل کردن حساب بعد از 5 تلاش ناموفق
                if (user.FailedLoginAttempts >= 5)
                {
                    user.LockoutEndDate = DateTime.UtcNow.AddMinutes(15);
                    await _userManager.UpdateAsync(user);
                    return BadRequest(new { message = "حساب شما به دلیل تلاش‌های ناموفق قفل شد" });
                }

                await _userManager.UpdateAsync(user);
                return BadRequest(new { 
                    message = "کد تایید اشتباه است",
                    remainingAttempts = 5 - user.FailedLoginAttempts
                });
            }

            // تایید موفق
            user.IsMobileVerified = true;
            user.PhoneNumberConfirmed = true;
            user.MobileVerificationCode = null;
            user.MobileVerificationCodeExpiry = null;
            user.FailedLoginAttempts = 0;
            user.LockoutEndDate = null;
            user.LastLoginDate = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            // ورود کاربر
            await _signInManager.SignInAsync(user, isPersistent: false);

            _logger.LogInformation("کاربر {PhoneNumber} با موفقیت وارد شد", request.PhoneNumber);

            return Ok(new { 
                message = "ورود موفقیت‌آمیز",
                user = new {
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
