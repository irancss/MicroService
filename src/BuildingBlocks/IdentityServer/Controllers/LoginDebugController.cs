using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginDebugController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<LoginDebugController> _logger;

    public LoginDebugController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<LoginDebugController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    [HttpGet("test-login")]
    public async Task<IActionResult> TestLogin(string phoneNumber = "09123456789", string password = "Admin123!")
    {
        var result = new
        {
            Step1_FindUser = "بررسی کاربر...",
            Step2_CheckPassword = "بررسی رمز عبور...",
            Step3_SignIn = "تلاش برای ورود...",
            Step4_IsSignedIn = "بررسی وضعیت ورود...",
            Errors = new List<string>()
        };

        try
        {
            // Step 1: Find user
            var user = await _userManager.FindByNameAsync(phoneNumber);
            if (user == null)
            {
                return Ok(new { Error = "کاربر یافت نشد", PhoneNumber = phoneNumber });
            }

            var step1 = $"کاربر یافت شد: ID={user.Id}, UserName={user.UserName}, IsActive={user.IsActive}";

            // Step 2: Check password
            var passwordValid = await _userManager.CheckPasswordAsync(user, password);
            var step2 = $"رمز عبور معتبر: {passwordValid}";

            // Step 3: Sign in attempt
            var signInResult = await _signInManager.PasswordSignInAsync(user, password, true, lockoutOnFailure: false);
            var step3 = $"ورود: موفق={signInResult.Succeeded}, قفل={signInResult.IsLockedOut}, نیاز تایید={signInResult.RequiresTwoFactor}";

            // Step 4: Check if signed in
            var isSignedIn = _signInManager.IsSignedIn(User);
            var step4 = $"وضعیت ورود فعلی: {isSignedIn}";

            // Additional info
            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                Step1_FindUser = step1,
                Step2_CheckPassword = step2,
                Step3_SignIn = step3,
                Step4_IsSignedIn = step4,
                User = new
                {
                    user.Id,
                    user.UserName,
                    user.PhoneNumber,
                    user.IsActive,
                    user.IsMobileVerified,
                    user.LockoutEnabled,
                    user.LockoutEnd,
                    user.AccessFailedCount,
                    Claims = userClaims.Select(c => $"{c.Type}: {c.Value}"),
                    Roles = userRoles
                },
                SignInResult = new
                {
                    signInResult.Succeeded,
                    signInResult.IsLockedOut,
                    signInResult.IsNotAllowed,
                    signInResult.RequiresTwoFactor
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در تست ورود");
            return Ok(new { Error = ex.Message, StackTrace = ex.StackTrace });
        }
    }

    [HttpGet("reset-user")]
    public async Task<IActionResult> ResetUser(string phoneNumber = "09123456789")
    {
        try
        {
            var user = await _userManager.FindByNameAsync(phoneNumber);
            if (user == null)
            {
                return Ok(new { Error = "کاربر یافت نشد" });
            }

            // Reset lockout
            await _userManager.SetLockoutEndDateAsync(user, null);
            await _userManager.ResetAccessFailedCountAsync(user);

            // Ensure user is active
            user.IsActive = true;
            await _userManager.UpdateAsync(user);

            return Ok(new { Success = "کاربر بازنشانی شد", UserId = user.Id });
        }
        catch (Exception ex)
        {
            return Ok(new { Error = ex.Message });
        }
    }
}
