using System.ComponentModel.DataAnnotations;

namespace User.Models
{
    public class RegisterModel
    {
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
