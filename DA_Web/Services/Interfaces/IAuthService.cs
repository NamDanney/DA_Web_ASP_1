using DA_Web.DTOs.Auth;
using DA_Web.DTOs.Common;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace DA_Web.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto);
        Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto registerDto);
        Task<ApiResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
        Task<ApiResponse<bool>> ForgotPasswordAsync(string email);
        Task<ApiResponse<bool>> VerifyOtpAsync(string email, string otp);
        Task<ApiResponse<bool>> ResetPasswordAsync(string email, string otp, string newPassword);
        Task<ApiResponse<UserInfoDto>> GetUserProfileAsync(int userId);
        Task<ApiResponse<bool>> UpdateUserProfileAsync(int userId, UpdateUserProfileDto updateDto);
        Task<bool> IsUsernameExistsAsync(string username);
        Task<bool> IsEmailExistsAsync(string email);
        Task<bool> IsPhoneExistsAsync(string phone);
        Task<ApiResponse<bool>> ResendOtpAsync(string email);
    }
}