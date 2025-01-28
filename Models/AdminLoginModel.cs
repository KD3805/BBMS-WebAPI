using System.ComponentModel.DataAnnotations;

namespace BBMS_WebAPI.Models
{
    public class AdminLoginModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
