using System.ComponentModel.DataAnnotations;

namespace User.Models
{
    public class LoginModel
    {
        [Required]
        [Phone] 
        public string PhoneNumber { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
