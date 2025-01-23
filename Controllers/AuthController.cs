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
        //private readonly DonorRepository _donorRepository;
        //private readonly TokenService _tokenService;

        //public AuthController(DonorRepository donorRepository, TokenService tokenService)
        //{
        //    _donorRepository = donorRepository;
        //    _tokenService = tokenService;
        //}

        //[HttpPost("VerifyDonor")]
        //public IActionResult VerifyDonor([FromBody] LoginRequestModel loginRequest)
        //{
        //    if (string.IsNullOrWhiteSpace(loginRequest.Email))
        //        return BadRequest(new { message = "Email is required." });

        //    try
        //    {
        //        var donor = _donorRepository.GetByEmail(loginRequest.Email);
        //        if (donor == null)
        //            return Unauthorized(new { message = "Donor not found." });

        //        return Ok(new
        //        {
        //            message = "Donor found successfully.",
        //            success=true
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "An error occurred during login.", error = ex.Message });
        //    }
        //}

    }
}
