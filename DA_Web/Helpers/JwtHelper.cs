using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DA_Web.Models;
using DA_Web.Configurations;
using Microsoft.Extensions.Options;

namespace DA_Web.Helpers
{
    public class JwtHelper
    {
        private readonly JwtConfig _jwtConfig;

        public JwtHelper(IOptions<JwtConfig> jwtConfig)
        {
            _jwtConfig = jwtConfig.Value;
        }

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtConfig.SecretKey);

            // Xử lý giá trị avatar: Nếu avatar null hoặc rỗng, dùng một đường dẫn mặc định
            var avatarUrl = !string.IsNullOrEmpty(user.Avatar)
                              ? user.Avatar
                              : "/uploads/avatars/default.png"; // <-- Đặt một ảnh mặc định trong wwwroot/images/avatars

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("FullName", user.FullName ?? ""), // Thêm ?? "" để tránh lỗi nếu FullName là null
                new Claim("avatar", avatarUrl) // <-- THÊM DÒNG NÀY
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // ... (phần còn lại của phương thức không đổi)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtConfig.SecretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _jwtConfig.Issuer,
                    ValidAudience = _jwtConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        public int? GetUserIdFromToken(string token)
        {
            var principal = ValidateToken(token);
            if (principal == null) return null;

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }

            return null;
        }
    }
}