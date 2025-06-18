using DA_Web.DTOs.Auth;
using DA_Web.DTOs.Common;
using DA_Web.Models;
using DA_Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DA_Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public ProfileController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userProfile = await _userService.GetUserProfile(User);
            if (userProfile == null) return RedirectToAction("Login", "Account");
            return View(userProfile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(UserInfoDto model, IFormFile? avatarFile)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId)) return Unauthorized();

            await _userService.UpdateUserProfileAsync(userId, new UpdateUserProfileDto { FullName = model.FullName, Phone = model.Phone });
            if (avatarFile != null)
            {
                await _userService.UpdateUserAvatarAsync(userId, avatarFile);
            }

            // --- LÀM MỚI COOKIE VỚI THÔNG TIN MỚI NHẤT ---
            var updatedUser = await _userService.GetUserByIdForClaimsAsync(userId);
            if (updatedUser != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, updatedUser.Id.ToString()),
                    new Claim(ClaimTypes.Name, updatedUser.Username),
                    new Claim(ClaimTypes.Email, updatedUser.Email),
                    new Claim(ClaimTypes.Role, updatedUser.Role.ToString()),
                    // Sửa lỗi ở đây: Đảm bảo đường dẫn avatar mới cũng có dấu "/"
                    new Claim("Avatar", string.IsNullOrEmpty(updatedUser.Avatar)
                                       ? "/images/default-avatar.png"
                                       : "/" + updatedUser.Avatar.Replace("\\", "/"))
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            }

            TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
            return RedirectToAction("Index");
        }

        // Phương thức ChangePassword giữ nguyên
        [HttpGet]
        public IActionResult ChangePassword() => View(new ChangePasswordDto());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            if (!ModelState.IsValid) return View(model);
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId)) return Unauthorized();
            var result = await _authService.ChangePasswordAsync(userId, model);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, result.Message ?? "Lỗi không xác định.");
            return View(model);
        }
    }
}