using BBMS_WebAPI.Data;
using BBMS_WebAPI.Services;
using BBMS_WebAPI.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BBMS_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OtpController : ControllerBase
    {
        private readonly OtpService _otpService;
        private readonly EmailHelper _emailHelper;
        private readonly DonorRepository _donorRepository;
        private readonly RecipientRepository _recipientRepository;
        private readonly AdminRepository _adminRepository;
        private readonly TokenService _tokenService;

        public OtpController(OtpService otpService, EmailHelper emailHelper, DonorRepository donorRepository, RecipientRepository recipientRepository, AdminRepository adminRepository, TokenService tokenService)
        {
            _otpService = otpService;
            _emailHelper = emailHelper;
            _donorRepository = donorRepository;
            _tokenService = tokenService;
            _recipientRepository = recipientRepository;
            _adminRepository = adminRepository; 
        }


        [HttpPost("SendOTP")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest rq)
        {
            if (string.IsNullOrEmpty(rq.Email))
                return BadRequest(new { success = false, message = "Email is required" });

            var otp = await _otpService.GenerateAndSaveOtp(rq.Email);
            var emailBody = $"<!DOCTYPE html>\r\n<html>\r\n<head>\r\n  <style>\r\n    body {{\r\n      font-family: Arial, sans-serif;\r\n      line-height: 1.6;\r\n      color: #333;\r\n      margin: 0;\r\n      padding: 0;\r\n    }}\r\n    .email-container {{\r\n      width: 100%;\r\n      max-width: 600px;\r\n      margin: 0 auto;\r\n      background: #f7f7f7;\r\n      padding: 20px;\r\n      border: 1px solid #ddd;\r\n      border-radius: 10px;\r\n    }}\r\n    .email-header {{\r\n      text-align: center;\r\n      padding: 10px 0;\r\n      background: linear-gradient(to right, #BE3F35, #CA3668);\r\n      color: #fff;\r\n      font-size: 24px;\r\n      border-radius: 8px;\r\n    }}\r\n    .email-body {{\r\n      margin: 20px 0;\r\n      text-align: center;\r\n    }}\r\n    .otp {{\r\n      font-size: 36px;\r\n      font-weight: bold;\r\n      color: #b91c1c;\r\n      margin: 10px 0;\r\n    }}\r\n    .email-footer {{\r\n      text-align: center;\r\n      font-size: 14px;\r\n      color: #999;\r\n    }}\r\n  </style>\r\n</head>\r\n<body>\r\n  <div class=\"email-container\">\r\n    <div class=\"email-header\">\r\n      Red Vault\r\n    </div>\r\n    <div class=\"email-body\">\r\n      <p>Dear User,</p>\r\n      <p>Here is your One-Time Password (OTP) to verify your email:</p>\r\n      <div class=\"otp\">{otp}</div>\r\n      <p>Please do not share this OTP to anyone. The OTP is valid for the next 5 minutes.</p>\r\n    </div>\r\n    <div class=\"email-footer\">\r\n      © 2024 ● Blood Bank Management System. All Rights Reserved.\r\n    </div>\r\n  </div>\r\n</body>\r\n</html>";

            await _emailHelper.SendEmail(rq.Email, "Red Vault OTP Code", emailBody);
            return Ok(new { success = true, message = "OTP sent successfully!" });  
        }

        #region Donor Login
        [HttpPost("VerifyOTP/Donor/Login")]
        public async Task<IActionResult> VerifyOtpAndDonorLogin([FromBody] OtpVerifyRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.OtpCode))
                return BadRequest(new { success = false, message = "Email and OTP are required" });

            // Retrieve donor details
            var donor = _donorRepository.GetByEmail(request.Email);
            if (donor == null)
                return BadRequest(new { success = false, message = "Donor not found." });

            var isValid = await _otpService.VerifyOtp(request.Email, request.OtpCode);
            if (!isValid)
                return BadRequest(new { success = false, message = "Invalid or expired OTP" });

            // Generate JWT token
            var token = _tokenService.GenerateToken(donor.DonorID, donor.Email);

            return Ok(new
            {
                success = true,
                message = "OTP verified successfully!",
                token,
                donorDetails = new
                {
                    donor.DonorID,
                    donor.Name,
                    donor.DOB,
                    donor.Age,
                    donor.Gender,
                    donor.BloodGroupName,
                    donor.Phone,
                    donor.Email,
                    donor.Address
                }
            });
        }
        #endregion


        #region Recipient Login
        [HttpPost("VerifyOTP/Recipient/Login")]
        public async Task<IActionResult> VerifyOtpAndRecipientLogin([FromBody] OtpVerifyRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.OtpCode))
                return BadRequest(new { success = false, message = "Email and OTP are required" });

            // Retrieve donor details
            var recipient = _recipientRepository.GetByEmail(request.Email);
            if (recipient == null)
                return BadRequest(new { success = false, message = "Recipient not found." });

            var isValid = await _otpService.VerifyOtp(request.Email, request.OtpCode);
            if (!isValid)
                return BadRequest(new { success = false, message = "Invalid or expired OTP" });

            // Generate JWT token
            var token = _tokenService.GenerateToken(recipient.RecipientID, recipient.Email);

            return Ok(new
            {
                success = true,
                message = "OTP verified successfully!",
                token,
                recipientDetails = new
                {
                    recipient.RecipientID,
                    recipient.Name,
                    recipient.DOB,
                    recipient.Age,
                    recipient.Gender,
                    recipient.BloodGroupName,
                    recipient.Phone,
                    recipient.Email,
                    recipient.Address
                }
            });
        }
        #endregion

        #region Admin Login
        [HttpPost("VerifyOTP/Admin/Login")]
        public async Task<IActionResult> VerifyOtpAndAdminLogin([FromBody] OtpVerifyRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.OtpCode))
                return BadRequest(new { success = false, message = "Email and OTP are required" });

            // Retrieve donor details
            var admin = _adminRepository.GetByEmail(request.Email);
            if (admin == null)
                return BadRequest(new { success = false, message = "Admin not found." });

            var isValid = await _otpService.VerifyOtp(request.Email, request.OtpCode);
            if (!isValid)
                return BadRequest(new { success = false, message = "Invalid or expired OTP" });

            // Generate JWT token
            var token = _tokenService.GenerateToken(admin.AdminID, admin.Email);

            return Ok(new
            {
                success = true,
                message = "OTP verified successfully!",
                token,
                adminDetails = new
                {
                    admin.AdminID,
                    admin.Name,
                    admin.Email,
                    admin.Phone,
                    admin.Role
                }   
            });
        }
        #endregion

    }

    public class SendOtpRequest
    {
        [Required(ErrorMessage="Email is required.")]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class OtpVerifyRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "OTP is required.")]
        [MinLength(6, ErrorMessage = "OTP must be exactly 6 digits.")]
        [MaxLength(6, ErrorMessage = "OTP must be exactly 6 digits.")]
        public string OtpCode { get; set; }
    }
}
