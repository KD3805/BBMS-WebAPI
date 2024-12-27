using BBMS_WebAPI.Data;
using BBMS_WebAPI.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BBMS_WebAPI.Services
{
    public class OtpService
    {
        private readonly DataContext _context;

        public OtpService(DataContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateAndSaveOtp(string email)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            var otpEntity = new OtpModel
            {
                Email = email,
                OtpCode = otp
            };

            _context.Otps.Add(otpEntity);
            await _context.SaveChangesAsync();

            return otp;
        }

        public async Task<bool> VerifyOtp(string email, string otpCode)
        {
            var otpEntity = _context.Otps
                .FirstOrDefault(o => o.Email == email && o.OtpCode == otpCode && o.ExpiresAt > DateTime.UtcNow);

            if (otpEntity != null)
            {
                _context.Otps.Remove(otpEntity);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
