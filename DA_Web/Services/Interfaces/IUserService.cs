using DA_Web.DTOs.Auth;
using DA_Web.DTOs.Common;
using DA_Web.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using DA_Web.Models.Enums;

namespace DA_Web.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserInfoDto?> GetUserProfile(ClaimsPrincipal user);
        Task<ApiResponse<bool>> UpdateUserProfileAsync(int userId, UpdateUserProfileDto model);
        Task<ApiResponse<string>> UpdateUserAvatarAsync(int userId, IFormFile avatarFile);
        Task<User?> GetUserByIdForClaimsAsync(int userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<ApiResponse<bool>> UpdateUserRoleAsync(int userId, RoleType newRole);
        Task<ApiResponse<bool>> DeleteUserAsync(int userId);
    }
}