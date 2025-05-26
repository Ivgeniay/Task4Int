using System.ComponentModel.DataAnnotations;
using UserManagement.Persistence;

namespace UserManagement.Web.Models
{
#pragma warning disable CS8618
    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(PersistentsConstants.EMAIL_LENGTH, ErrorMessage = $"Email is too long.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(1, ErrorMessage = "Password must be more than 1 character")]
        public string Password { get; set; }
    }
#pragma warning restore CS8618
}
