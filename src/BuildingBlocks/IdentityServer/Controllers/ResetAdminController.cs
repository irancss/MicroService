using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers;

public class ResetAdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ResetAdminController> _logger;

    public ResetAdminController(UserManager<ApplicationUser> userManager, ILogger<ResetAdminController> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> ResetAdminPassword()
    {
        var user = await _userManager.FindByNameAsync("09123456789");
        if (user == null)
        {
            return Json(new { success = false, message = "کاربر ادمین یافت نشد" });
        }

        // حذف رمز عبور فعلی
        var removeResult = await _userManager.RemovePasswordAsync(user);
        if (!removeResult.Succeeded)
        {
            return Json(new { success = false, message = "خطا در حذف رمز عبور قبلی", errors = removeResult.Errors });
        }

        // افزودن رمز عبور جدید
        var addResult = await _userManager.AddPasswordAsync(user, "Admin123!");
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
                <button onclick='resetPassword()'>Reset Admin Password to Admin123!</button>
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
