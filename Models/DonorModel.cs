using System.ComponentModel.DataAnnotations;

namespace BBMS_WebAPI.Models
{
    public class DonorModel
    {
        public int DonorID { get; set; } // Optional for insert

        public string Name { get; set; } // Required

        public DateTime DOB { get; set; } // Required

        public int Age { get; set; } // Required

        public string Gender { get; set; } // Required

        public string BloodGroup { get; set; } // Required

        public string Phone { get; set; } // Required

        public string Email { get; set; } // Required

        public string Address { get; set; } // Required

        public DateTime CreatedAt { get; set; } // Auto-set in DB

        public DateTime? UpdatedAt { get; set; } // Optional

        public bool IsActive { get; set; } // Added for soft delete
    }

    public class LoginRequestModel
    {
        [Required(ErrorMessage = "The email field is required.")]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class DonorDropDownModel
    {
        public int DonorID { get; set; }
        public string Name { get; set; }
    }
}
