using IdentityServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        _logger.LogInformation("تلاش ورود با شماره: {PhoneNumber}", model.PhoneNumber);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("مدل نامعتبر برای ورود: {Errors}", string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));
            return View(model);
        }

        var user = await _userManager.FindByNameAsync(model.PhoneNumber);
        if (user == null)
        {
            _logger.LogWarning("کاربر با شماره {PhoneNumber} یافت نشد", model.PhoneNumber);
            ModelState.AddModelError(string.Empty, "کاربری با این شماره موبایل یافت نشد.");
            return View(model);
        }

        _logger.LogInformation("کاربر پیدا شد: {UserId}, فعال: {IsActive}, قفل شده تا: {LockoutEnd}", user.Id, user.IsActive, user.LockoutEnd);

        // بررسی صحت رمز عبور
        var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
        _logger.LogInformation("رمز عبور معتبر: {PasswordValid}", passwordValid);

        if (!passwordValid)
        {
            _logger.LogWarning("رمز عبور نامعتبر برای کاربر {UserId}", user.Id);
            ModelState.AddModelError(string.Empty, "شماره موبایل یا رمز عبور اشتباه است.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);
        
        _logger.LogInformation("نتیجه ورود: موفق={Succeeded}, قفل={IsLockedOut}, نیاز به تایید={RequiresTwoFactor}, مجاز نیست={IsNotAllowed}", 
            result.Succeeded, result.IsLockedOut, result.RequiresTwoFactor, result.IsNotAllowed);

        if (result.Succeeded)
        {
            _logger.LogInformation("کاربر {UserId} وارد شد", user.Id);
            
            // بررسی وضعیت ورود
            var isSignedIn = _signInManager.IsSignedIn(User);
            _logger.LogInformation("وضعیت ورود پس از موفقیت: {IsSignedIn}", isSignedIn);
            
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                _logger.LogInformation("ریدایرکت به: {ReturnUrl}", returnUrl);
                return Redirect(returnUrl);
            }
            
            _logger.LogInformation("ریدایرکت به admin");
            return Redirect("/admin");
        }

        if (result.IsLockedOut)
        {
            _logger.LogWarning("حساب کاربری قفل شده: {UserId}", user.Id);
            ModelState.AddModelError(string.Empty, "حساب کاربری قفل شده است.");
        }
        else if (result.IsNotAllowed)
        {
            _logger.LogWarning("ورود مجاز نیست: {UserId}", user.Id);
            ModelState.AddModelError(string.Empty, "ورود مجاز نیست.");
        }
        else
        {
            _logger.LogWarning("ورود ناموفق برای کاربر {UserId}", user.Id);
            ModelState.AddModelError(string.Empty, "شماره موبایل یا رمز عبور اشتباه است.");
        }

        return View(model);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("کاربر خروج کرد");
        return RedirectToAction("Index", "Home");
    }
}
