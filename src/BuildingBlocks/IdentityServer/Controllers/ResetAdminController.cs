using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers;

#if DEBUG
public class ResetAdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ResetAdminController> _logger;
       private readonly IConfiguration _configuration;

    public ResetAdminController(UserManager<ApplicationUser> userManager, ILogger<ResetAdminController> logger, IConfiguration configuration    )
    {
        _userManager = userManager;
        _logger = logger;
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<IActionResult> ResetAdminPassword()
    {
        var user = await _userManager.FindByNameAsync("09124607630");
        if (user == null)
        {
            return Json(new { success = false, message = "کاربر ادمین یافت نشد" });
        }

        // حذف رمز عبور فعلی
         var newPassword = _configuration["AdminUser:DefaultPassword"];
         if (string.IsNullOrEmpty(newPassword))
            return Json(new { success = false, message = "رمز عبور پیش‌فرض در تنظیمات یافت نشد" });

        var removeResult = await _userManager.RemovePasswordAsync(user);
        if (!removeResult.Succeeded)
        {
            return Json(new { success = false, message = "خطا در حذف رمز عبور قبلی", errors = removeResult.Errors });
        }

        // افزودن رمز عبور جدید
        var addResult = await _userManager.AddPasswordAsync(user, newPassword);
        if (addResult.Succeeded)
        {
            _logger.LogInformation("رمز عبور ادمین reset شد");
            return Json(new { success = true, message = "رمز عبور ادمین با موفقیت reset شد" });
        }

        return Json(new { success = false, message = "خطا در تنظیم رمز عبور جدید", errors = addResult.Errors });
    }

    [HttpGet]
    public IActionResult Index()
    {
        return Content(@"
            <html>
            <body>
                <h2>Reset Admin Password</h2>
                <button onclick='resetPassword()'>Reset Admin Password to </button>
                <div id='result'></div>
                <script>
                    function resetPassword() {
                        fetch('/ResetAdmin/ResetAdminPassword', { method: 'POST' })
                        .then(response => response.json())
                        .then(data => {
                            document.getElementById('result').innerHTML = JSON.stringify(data, null, 2);
                        });
                    }
                </script>
            </body>
            </html>
        ", "text/html");
    }
}
#endif
