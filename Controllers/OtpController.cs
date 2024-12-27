using BBMS_WebAPI.Services;
using BBMS_WebAPI.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace BBMS_WebAPI.Controllers
{
    [Route("api/otp")]
    [ApiController]
    public class OtpController : ControllerBase
    {
        private readonly OtpService _otpService;
        private readonly EmailHelper _emailHelper;

        public OtpController(OtpService otpService, EmailHelper emailHelper)
        {
            _otpService = otpService;
            _emailHelper = emailHelper;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest rq)
        {
            if (string.IsNullOrEmpty(rq.Email))
                return BadRequest(new { success = false, message = "Email is required" });

            var otp = await _otpService.GenerateAndSaveOtp(rq.Email);
            var emailBody = $"<!DOCTYPE html>\r\n<html>\r\n<head>\r\n  <style>\r\n    body {{\r\n      font-family: Arial, sans-serif;\r\n      line-height: 1.6;\r\n      color: #333;\r\n      margin: 0;\r\n      padding: 0;\r\n    }}\r\n    .email-container {{\r\n      width: 100%;\r\n      max-width: 600px;\r\n      margin: 0 auto;\r\n      background: #f7f7f7;\r\n      padding: 20px;\r\n      border: 1px solid #ddd;\r\n      border-radius: 10px;\r\n    }}\r\n    .email-header {{\r\n      text-align: center;\r\n      padding: 10px 0;\r\n      background-color: #007BFF;\r\n      color: #fff;\r\n      font-size: 24px;\r\n      border-radius: 8px;\r\n    }}\r\n    .email-body {{\r\n      margin: 20px 0;\r\n      text-align: center;\r\n    }}\r\n    .otp {{\r\n      font-size: 36px;\r\n      font-weight: bold;\r\n      color: #007BFF;\r\n      margin: 10px 0;\r\n    }}\r\n    .email-footer {{\r\n      text-align: center;\r\n      font-size: 14px;\r\n      color: #999;\r\n    }}\r\n  </style>\r\n</head>\r\n<body>\r\n  <div class=\"email-container\">\r\n    <div class=\"email-header\">\r\n      Blood Bank Management System\r\n    </div>\r\n    <div class=\"email-body\">\r\n      <p>Dear User,</p>\r\n      <p>Here is your One-Time Password (OTP) to verify your email:</p>\r\n      <div class=\"otp\">{otp}</div>\r\n      <p>Please use this OTP to complete your verification process. The OTP is valid for the next 5 minutes.</p>\r\n    </div>\r\n    <div class=\"email-footer\">\r\n      © 2024 ● Blood Bank Management System. All Rights Reserved.\r\n    </div>\r\n  </div>\r\n</body>\r\n</html>";

            await _emailHelper.SendEmail(rq.Email, "Red Vault OTP Code", emailBody);
            return Ok(new { success = true, message = "OTP sent successfully!" });  
        }
                                                                                             
        [HttpPost("verify")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyRequest request)
        {
            Console.WriteLine($"Received Email: {request.Email}, OTP: {request.OtpCode}");

            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.OtpCode))
                return BadRequest(new { success = false, message = "Email and OTP are required" });

            var isValid = await _otpService.VerifyOtp(request.Email, request.OtpCode);

            if (!isValid)
                return BadRequest(new { success = false, message = "Invalid or expired OTP" });

            return Ok(new { success = true, message = "OTP verified successfully!" });
        }
    }

    public class SendOtpRequest
    {
        public string Email { get; set; }
    }

    public class OtpVerifyRequest
    {
        public string Email { get; set; }
        public string OtpCode { get; set; }
    }
}
