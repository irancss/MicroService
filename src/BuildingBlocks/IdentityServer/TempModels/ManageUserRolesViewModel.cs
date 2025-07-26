using System.ComponentModel.DataAnnotations;

namespace TempModels;

public class ManageUserRolesViewModel
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public List<UserRoleViewModel> Roles { get; set; }
}

public class UserRoleViewModel
{
    public string RoleName { get; set; }
    public bool IsSelected { get; set; }
}

public class ManageUserClaimsViewModel
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public List<Claim> ExistingClaims { get; set; } = new List<Claim>();
    
    [Required]
    public string ClaimType { get; set; }
    [Required]
    public string ClaimValue { get; set; }
}