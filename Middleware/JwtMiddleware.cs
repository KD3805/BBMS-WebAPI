using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;

namespace BBMS_WebAPI.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _jwtSecret;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _jwtSecret = configuration["Jwt:Key"];
        }

        public async Task Invoke(HttpContext context)
        {
            // Define public endpoints that do not require authentication
            var publicEndpoints = new[] {
                "/api/Donor/Email",
                "/api/Admin/Email",
                "/api/Recipient/Email",
                "/api/Otp/SendOTP",
                "/api/Otp/VerifyOTP/Donor/Login",
                "/api/Otp/VerifyOTP/Recipient/Login",
                "/api/Otp/VerifyOTP/Admin/Login",
                "/api/Admin/Login"
            };

            // Check if the current path is a public endpoint
            var currentPath = context.Request.Path.Value; // Get the request path
            if (publicEndpoints.Any(e => e.Equals(currentPath, StringComparison.OrdinalIgnoreCase)))
            {
                // Skip token validation for public endpoints
                await _next(context);
                return;
            }

            // Validate token for secured endpoints
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    AttachUserToContext(context, token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Token validation failed: {ex.Message}");
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid or expired token.");
                    return; // Terminate pipeline if token is invalid
                }
            }
            else
            {
                // If no token is provided and it's not a public endpoint, return Unauthorized
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token is required.");
                return;
            }

            await _next(context); // Proceed to the next middleware
        }

        private void AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtSecret);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    throw new SecurityTokenException("UserId claim missing from token.");
                }

                context.Items["UserId"] = int.Parse(userId); // Attach UserId to context
            }
            catch (SecurityTokenExpiredException)
            {
                throw new SecurityTokenException("Token has expired.");
            }
            catch (Exception)
            {
                throw new SecurityTokenException("Invalid token.");
            }
        }
    }
}
