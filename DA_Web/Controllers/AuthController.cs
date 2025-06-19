using DA_Web.DTOs.Auth;
using DA_Web.DTOs.Common;
using DA_Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DA_Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// User login
        /// </summary>
        /// <param name="loginDto">Login credentials</param>
        /// <returns>JWT token and user info</returns>
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<AuthResponseDto>.ErrorResult("Validation failed", errors));
            }

            var result = await _authService.LoginAsync(loginDto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


        /// <summary>
        /// Verify user account using OTP
        /// </summary>
        /// <param name="verifyDto">OTP verification data</param>
        /// <returns>Success status</returns>
        [HttpPost("VerifyOtp")]
        public async Task<ActionResult<ApiResponse<bool>>> VerifyOtp([FromBody] VerifyOtpDto verifyDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<bool>.ErrorResult("Validation failed", errors));
            }

            var result = await _authService.VerifyOtpAsync(verifyDto.Email, verifyDto.Otp);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }



        /// <summary>
        /// Resend OTP for account verification
        /// </summary>
        /// <param name="resendDto">Object containing user's email</param>
        /// <returns>Success status</returns>
        [HttpPost("ResendOtp")]
        public async Task<ActionResult<ApiResponse<bool>>> ResendOtp([FromBody] ResendOtpDto resendDto)
        {
            if (string.IsNullOrWhiteSpace(resendDto.Email))
            {
                return BadRequest(ApiResponse<bool>.ErrorResult("Email is required."));
            }

            var result = await _authService.ResendOtpAsync(resendDto.Email);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// User registration
        /// </summary>
        /// <param name="registerDto">Registration data</param>
        /// <returns>JWT token and user info</returns>
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<AuthResponseDto>.ErrorResult("Validation failed", errors));
            }

            var result = await _authService.RegisterAsync(registerDto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Change user password
        /// </summary>
        /// <param name="changePasswordDto">Password change data</param>
        /// <returns>Success status</returns>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<bool>.ErrorResult("Validation failed", errors));
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(ApiResponse<bool>.ErrorResult("User not authenticated"));
            }

            var result = await _authService.ChangePasswordAsync(userId.Value, changePasswordDto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Forgot password - send OTP to email
        /// </summary>
        /// <param name="dto">User email</param>
        /// <returns>Success status</returns>
        [HttpPost("ForgotPassword")] // <-- SỬA TÊN ROUTE
        public async Task<ActionResult<ApiResponse<bool>>> ForgotPassword([FromBody] ForgotPasswordDto dto) // <-- SỬA THAM SỐ
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                return BadRequest(ApiResponse<bool>.ErrorResult("Email is required"));
            }

            // Gọi service với dto.Email
            var result = await _authService.ForgotPasswordAsync(dto.Email);
            return Ok(result);
        }

        /// <summary>
        /// Reset password using OTP
        /// </summary>
        /// <param name="resetDto">Reset password data</param>
        /// <returns>Success status</returns>
        [HttpPost("ResetPassword")]
        public async Task<ActionResult<ApiResponse<bool>>> ResetPassword([FromBody] ResetPasswordDto resetDto) // <-- SỬ DỤNG DTO CÓ SẴN CỦA BẠN
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<bool>.ErrorResult("Validation failed", errors));
            }

            // Logic gọi service không thay đổi
            var result = await _authService.ResetPasswordAsync(resetDto.Email, resetDto.Otp, resetDto.NewPassword);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        /// <returns>User profile information</returns>
        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UserInfoDto>>> GetProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(ApiResponse<UserInfoDto>.ErrorResult("User not authenticated"));
            }

            var result = await _authService.GetUserProfileAsync(userId.Value);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Update user profile
        /// </summary>
        /// <param name="updateDto">Profile update data</param>
        /// <returns>Success status</returns>
        [HttpPut("profile")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateProfile([FromBody] UpdateUserProfileDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<bool>.ErrorResult("Validation failed", errors));
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(ApiResponse<bool>.ErrorResult("User not authenticated"));
            }

            var result = await _authService.UpdateUserProfileAsync(userId.Value, updateDto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Check if username exists
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <returns>True if exists</returns>
        [HttpGet("check-username/{username}")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckUsername(string username)
        {
            var exists = await _authService.IsUsernameExistsAsync(username);
            return Ok(ApiResponse<bool>.SuccessResult(exists, exists ? "Username exists" : "Username available"));
        }

        /// <summary>
        /// Check if email exists
        /// </summary>
        /// <param name="email">Email to check</param>
        /// <returns>True if exists</returns>
        [HttpGet("check-email/{email}")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckEmail(string email)
        {
            var exists = await _authService.IsEmailExistsAsync(email);
            return Ok(ApiResponse<bool>.SuccessResult(exists, exists ? "Email exists" : "Email available"));
        }

        /// <summary>
        /// Check if phone exists
        /// </summary>
        /// <param name="phone">Phone to check</param>
        /// <returns>True if exists</returns>
        [HttpGet("check-phone/{phone}")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckPhone(string phone)
        {
            var exists = await _authService.IsPhoneExistsAsync(phone);
            return Ok(ApiResponse<bool>.SuccessResult(exists, exists ? "Phone exists" : "Phone available"));
        }

        /// <summary>
        /// Logout (client-side token removal)
        /// </summary>
        /// <returns>Success message</returns>
        [HttpPost("logout")]
        [Authorize]
        public ActionResult<ApiResponse<bool>> Logout()
        {
            // JWT is stateless, so logout is handled on client-side by removing the token
            return Ok(ApiResponse<bool>.SuccessResult(true, "Logged out successfully"));
        }

        /// <summary>
        /// Get current user ID from JWT token
        /// </summary>
        /// <returns>User ID or null</returns>
        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }
    }
}