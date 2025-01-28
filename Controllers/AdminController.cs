using BBMS_WebAPI.Data;
using BBMS_WebAPI.Models;
using BBMS_WebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BBMS_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AdminRepository _adminRepository;
        private readonly TokenService _tokenService;

        public AdminController(IConfiguration configuration, TokenService tokenService)
        {
            _adminRepository = new AdminRepository(configuration);
            _tokenService = tokenService;
        }

        #region GetAll
        [HttpGet]
        public IActionResult GetAllAdmins()
        {
            var admins = _adminRepository.GetAll();
            if (admins == null || admins.Count == 0)
                return NotFound("No admins found.");

            return Ok(admins);
        }
        #endregion

        #region GetById
        [HttpGet("{id:int}")]
        public IActionResult GetAdminById(int id)
        {
            var admin = _adminRepository.GetById(id);
            if (admin == null)
                return NotFound($"Admin with ID {id} not found.");

            return Ok(admin);
        }
        #endregion

        #region Admin Profile
        [HttpGet("Profile")]
        public IActionResult GetAdminProfile()
        {
            var userId = HttpContext.Items["UserId"];

            if (userId == null)
            {
                Console.WriteLine("Unauthorized: Token validation failed or user not attached to context.");
                return Unauthorized("Invalid or expired token.");
            }

            if (userId == null)
                return Unauthorized("Invalid or expired token.");

            var admin = _adminRepository.GetById((int)userId);
            if (admin == null)
                return NotFound("Admin not found.");

            return Ok(new
            {
                admin.AdminID,
                admin.Name,
                admin.Email,
                admin.Phone,
                admin.Role
            });
        }
        #endregion

        #region Insert
        [HttpPost]
        public IActionResult InsertAdmin([FromBody] AdminModel adminModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Hash the password before inserting
                adminModel.Password = BCrypt.Net.BCrypt.HashPassword(adminModel.Password);
                var isInserted = _adminRepository.Insert(adminModel);

                if (isInserted)
                    return CreatedAtAction(nameof(GetAdminById), new { id = adminModel.AdminID }, adminModel);

                return StatusCode(StatusCodes.Status500InternalServerError, "Error inserting admin.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }
        #endregion

        #region Update
        [HttpPut("{id:int}")]
        public IActionResult UpdateAdmin(int id, [FromBody] AdminModel adminModel)
        {
            if (id != adminModel.AdminID)
                return BadRequest("Admin ID mismatch.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var admin = _adminRepository.GetById(id);
            if (admin == null)
                return NotFound($"Admin with ID {id} not found.");

            var isUpdated = _adminRepository.Update(adminModel);
            if (isUpdated)
                return NoContent();

            return StatusCode(StatusCodes.Status500InternalServerError, "Error updating admin.");
        }
        #endregion

        #region Delete
        [HttpDelete("{id:int}")]
        public IActionResult DeleteAdmin(int id)
        {
            var admin = _adminRepository.GetById(id);
            if (admin == null)
                return NotFound($"Admin with ID {id} not found.");

            var isDeleted = _adminRepository.Delete(id);
            if (isDeleted)
                return NoContent();

            return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting admin.");
        }
        #endregion

        #region Login
        [HttpPost("Login")]
        public IActionResult LoginAdmin([FromBody] LoginRequestModel loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Password))
                return BadRequest("Email and password are required.");

            try
            {
                var admin = _adminRepository.Login(loginRequest.Email, loginRequest.Password);
                if (admin == null)
                    return Unauthorized(new
                    {
                        exists=false,
                        message= "Invalid email or password. Please try again with valid credentials."
                    });

                // Generate JWT Token
                //var token = _tokenService.GenerateToken(admin.AdminID, admin.Email);

                return Ok(new
                {
                    message = "Admin found successfully",
                    exists=true,
                    //token,
                    //adminDetails = new
                    //{
                    //    admin.AdminID,
                    //    admin.Name,
                    //    admin.Email,
                    //    admin.Phone,
                    //    admin.Role
                    //}
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }
        #endregion

        #region GetByEmail
        [HttpPost("Email")]
        public IActionResult GetAdminByEmail([FromBody] LoginRequestModel request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest("Email is required.");

            var admin = _adminRepository.GetByEmail(request.Email);
            if (admin == null)
                return Ok(new
                {
                    exists = false,
                    message = "Admin with the provided email not found."
                });

            return Ok(new
            {
                message = "Admin email is already registered.",
                exists = true
            });
        }
        #endregion
    }
}
