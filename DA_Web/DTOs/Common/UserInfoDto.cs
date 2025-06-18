using DA_Web.Models.Enums;
using DA_Web.DTOs.Auth;
namespace DA_Web.DTOs.Common
{
    public class UserInfoDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public string? Avatar { get; set; }
        public RoleType Role { get; set; }
    }
}