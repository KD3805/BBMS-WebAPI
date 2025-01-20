using BBMS_WebAPI.Data;
using BBMS_WebAPI.Models;
using BBMS_WebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BBMS_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DonorRepository _donorRepository;
        private readonly TokenService _tokenService;

        public AuthController(DonorRepository donorRepository, TokenService tokenService)
        {
            _donorRepository = donorRepository;
            _tokenService = tokenService;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginRequestModel loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.Email))
                return BadRequest("Email is required.");

            var donor = _donorRepository.GetByEmail(loginRequest.Email);
            if (donor == null)
                return Unauthorized("Invalid email or password.");

            // Generate JWT Token
            var token = _tokenService.GenerateToken(donor.DonorID, donor.Email);

            return Ok(new
            {
                message = "Login successful.",
                token
            });
        }
    }
}
