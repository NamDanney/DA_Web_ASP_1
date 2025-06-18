using DA_Web.Data;
using DA_Web.DTOs.Auth;
using DA_Web.DTOs.Common; // <-- UserInfoDto sẽ được lấy từ đây
using DA_Web.Helpers;
using DA_Web.Models;
using DA_Web.Models.Enums;
using DA_Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DA_Web.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtHelper _jwtHelper;
        private readonly IEmailService _emailService;

        public AuthService(ApplicationDbContext context, JwtHelper jwtHelper, IEmailService emailService)
        {
            _context = context;
            _jwtHelper = jwtHelper;
            _emailService = emailService;
        }

        private string GenerateOtp()
        {
            return new Random().Next(100000, 999999).ToString();
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == loginDto.Username || u.Email == loginDto.Username);

                if (user == null || !PasswordHelper.VerifyPassword(loginDto.Password, user.PasswordHash))
                {
                    return ApiResponse<AuthResponseDto>.ErrorResult("Invalid username or password");
                }

                var token = _jwtHelper.GenerateToken(user);

                var response = new AuthResponseDto
                {
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(1440),
                    User = new UserInfoDto // <-- Đã hết lỗi
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        Phone = user.Phone, // Bổ sung Phone
                        FullName = user.FullName,
                        Role = user.Role,
                        Avatar = user.Avatar
                    }
                };
                return ApiResponse<AuthResponseDto>.SuccessResult(response, "Login successful");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login Error: {ex.Message}");
                return ApiResponse<AuthResponseDto>.ErrorResult("An error occurred during login.");
            }
        }

        public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto registerDto)
        {
            var validationErrors = new List<string>();
            if (await IsUsernameExistsAsync(registerDto.Username))
                validationErrors.Add("Username already exists");
            if (await IsEmailExistsAsync(registerDto.Email))
                validationErrors.Add("Email already exists");
            if (await IsPhoneExistsAsync(registerDto.Phone))
                validationErrors.Add("Phone number already exists");

            var passwordErrors = PasswordHelper.ValidatePasswordStrength(registerDto.Password);
            validationErrors.AddRange(passwordErrors);

            if (validationErrors.Any())
            {
                return ApiResponse<AuthResponseDto>.ErrorResult("Registration failed", validationErrors);
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var user = new User
                    {
                        Username = registerDto.Username,
                        Email = registerDto.Email,
                        Phone = registerDto.Phone,
                        PasswordHash = PasswordHelper.HashPassword(registerDto.Password),
                        FullName = registerDto.FullName,
                        Role = RoleType.user,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    var otp = GenerateOtp();
                    var userOtp = new UserOtp
                    {
                        Email = user.Email,
                        Otp = otp,
                        CreatedAt = DateTime.UtcNow,
                        ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                        Used = false
                    };
                    _context.UserOtps.Add(userOtp);
                    await _context.SaveChangesAsync();

                    var emailSubject = "Xác thực tài khoản của bạn - Phú Yên Travel";
                    var emailMessage = $"<p>Cảm ơn bạn đã đăng ký.</p><p>Mã OTP của bạn là: <strong>{otp}</strong></p><p>Mã này sẽ hết hạn sau 5 phút.</p>";
                    await _emailService.SendEmailAsync(user.Email, emailSubject, emailMessage);

                    await transaction.CommitAsync();

                    return ApiResponse<AuthResponseDto>.SuccessResult(null, "Registration successful. Please check your email for OTP to verify your account.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Registration Error: {ex.Message}");
                    return ApiResponse<AuthResponseDto>.ErrorResult("An error occurred during registration.");
                }
            }
        }

        public async Task<ApiResponse<UserInfoDto>> GetUserProfileAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return ApiResponse<UserInfoDto>.ErrorResult("User not found");

                var userInfo = new UserInfoDto 
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Phone = user.Phone,
                    FullName = user.FullName,
                    Role = user.Role,
                    Avatar = user.Avatar
                };
                return ApiResponse<UserInfoDto>.SuccessResult(userInfo, "User profile retrieved successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get Profile Error: {ex.Message}");
                return ApiResponse<UserInfoDto>.ErrorResult("An error occurred while retrieving profile.");
            }
        }

        public async Task<ApiResponse<bool>> VerifyOtpAsync(string email, string otp)
        {
            try
            {
                var userOtp = await _context.UserOtps
                    .FirstOrDefaultAsync(o => o.Email == email && o.Otp == otp && !o.Used && o.ExpiresAt > DateTime.UtcNow);

                if (userOtp == null)
                {
                    return ApiResponse<bool>.ErrorResult("Mã OTP không hợp lệ hoặc đã hết hạn.");
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return ApiResponse<bool>.ErrorResult("Không tìm thấy người dùng được liên kết với email này.");
                }

                userOtp.Used = true;
                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResult(true, "Tài khoản đã được xác thực thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Verify OTP Error: {ex.Message}");
                return ApiResponse<bool>.ErrorResult("Đã có lỗi xảy ra trong quá trình xác thực.");
            }
        }

        public async Task<ApiResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return ApiResponse<bool>.ErrorResult("User not found");

                // Sử dụng CurrentPassword đã được đổi tên ở DTO
                if (!PasswordHelper.VerifyPassword(changePasswordDto.CurrentPassword, user.PasswordHash))
                {
                    return ApiResponse<bool>.ErrorResult("Mật khẩu hiện tại không đúng.");
                }

                user.PasswordHash = PasswordHelper.HashPassword(changePasswordDto.NewPassword);
                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResult(true, "Đổi mật khẩu thành công.");
            }
            catch (Exception ex)
            {
                // Log lỗi
                return ApiResponse<bool>.ErrorResult("Đã có lỗi xảy ra khi đổi mật khẩu.");
            }
        }

        public async Task<ApiResponse<bool>> ForgotPasswordAsync(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return ApiResponse<bool>.SuccessResult(true, "If an account with that email exists, a password reset OTP has been sent.");
                }

                var otp = GenerateOtp();
                var userOtp = new UserOtp
                {
                    Email = email,
                    Otp = otp,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                    Used = false
                };
                _context.UserOtps.Add(userOtp);
                await _context.SaveChangesAsync();

                var emailSubject = "Yêu cầu đặt lại mật khẩu - Phú Yên Travel";
                var emailMessage = $"<p>Bạn đã yêu cầu đặt lại mật khẩu.</p><p>Mã OTP của bạn là: <strong>{otp}</strong></p><p>Mã này sẽ hết hạn sau 15 phút.</p>";
                await _emailService.SendEmailAsync(email, emailSubject, emailMessage);

                return ApiResponse<bool>.SuccessResult(true, "If an account with that email exists, a password reset OTP has been sent.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Forgot Password Error: {ex.Message}");
                return ApiResponse<bool>.ErrorResult("An error occurred.");
            }
        }

        public async Task<ApiResponse<bool>> ResetPasswordAsync(string email, string otp, string newPassword)
        {
            try
            {
                var userOtp = await _context.UserOtps
                    .FirstOrDefaultAsync(o => o.Email == email && o.Otp == otp && !o.Used && o.ExpiresAt > DateTime.UtcNow);

                if (userOtp == null) return ApiResponse<bool>.ErrorResult("Invalid or expired OTP");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null) return ApiResponse<bool>.ErrorResult("User not found");

                var passwordErrors = PasswordHelper.ValidatePasswordStrength(newPassword);
                if (passwordErrors.Any())
                {
                    return ApiResponse<bool>.ErrorResult("Password validation failed", passwordErrors);
                }

                user.PasswordHash = PasswordHelper.HashPassword(newPassword);
                user.UpdatedAt = DateTime.UtcNow;
                userOtp.Used = true;
                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResult(true, "Password reset successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Reset Password Error: {ex.Message}");
                return ApiResponse<bool>.ErrorResult("An error occurred while resetting password.");
            }
        }

        public async Task<ApiResponse<bool>> UpdateUserProfileAsync(int userId, UpdateUserProfileDto updateDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return ApiResponse<bool>.ErrorResult("User not found");

                if (await _context.Users.AnyAsync(u => u.Phone == updateDto.Phone && u.Id != userId))
                {
                    return ApiResponse<bool>.ErrorResult("Phone number already exists");
                }

                user.FullName = updateDto.FullName;
                user.Phone = updateDto.Phone;
                // user.Avatar = updateDto.Avatar; // Tạm thời comment lại, sẽ xử lý upload file sau
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResult(true, "Profile updated successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update Profile Error: {ex.Message}");
                return ApiResponse<bool>.ErrorResult("An error occurred while updating profile.");
            }
        }

        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> IsPhoneExistsAsync(string phone)
        {
            return await _context.Users.AnyAsync(u => u.Phone == phone);
        }
    }
}