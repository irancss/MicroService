using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "شماره موبایل الزامی است")]
    [Display(Name = "شماره موبایل")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "رمز عبور الزامی است")]
    [DataType(DataType.Password)]
    [Display(Name = "رمز عبور")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "مرا به خاطر بسپار")]
    public bool RememberMe { get; set; }
}
