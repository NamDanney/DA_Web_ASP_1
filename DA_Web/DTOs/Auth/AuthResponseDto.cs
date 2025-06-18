using DA_Web.Models.Enums;
using DA_Web.DTOs.Common;

namespace DA_Web.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserInfoDto User { get; set; } = new UserInfoDto();
    }
}