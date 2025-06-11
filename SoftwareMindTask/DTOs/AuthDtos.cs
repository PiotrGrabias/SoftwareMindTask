using System.ComponentModel.DataAnnotations;

namespace SoftwareMindTask.DTOs
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class LoginDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
