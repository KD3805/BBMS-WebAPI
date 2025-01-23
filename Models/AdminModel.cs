namespace BBMS_WebAPI.Models
{
    public class AdminModel
    {
        public int AdminID { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; } 

        public string Email { get; set; } 

        public string Password { get; set; } 

        public string Role { get; set; } = "Admin"; 

        public DateTime CreatedAt { get; set; } 

        public DateTime? UpdatedAt { get; set; }

    }
}
