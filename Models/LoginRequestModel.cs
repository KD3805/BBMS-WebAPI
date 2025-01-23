using System.ComponentModel.DataAnnotations;

namespace BBMS_WebAPI.Models
{
    public class LoginRequestModel
    {
        [Required(ErrorMessage = "The email field is required.")]
        [EmailAddress]
        public string Email { get; set; }

        public string? Password { get; set; }
    }
}
