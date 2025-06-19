using DA_Web.DTOs.Auth;
using DA_Web.Models;
using DA_Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DA_Web.Controllers
{
    public class AccountController : Controller
    {
        // Sử dụng lại IAuthService của bạn nhưng cho mục đích xác thực người dùng
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Gọi LoginAsync của bạn để kiểm tra username/password
            var authResult = await _authService.LoginAsync(model);

            if (authResult.Success && authResult.Data != null)
            {
                var user = authResult.Data.User;

                // --- TẠO CLAIMS VỚI ĐƯỜNG DẪN AVATAR ĐÚNG ---
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    // Sửa lỗi ở đây: Đảm bảo đường dẫn luôn bắt đầu bằng "/"
                    new Claim("Avatar", string.IsNullOrEmpty(user.Avatar)
                                       ? "/images/default-avatar.png"
                                       : "/" + user.Avatar.Replace("\\", "/"))
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, // Ghi nhớ đăng nhập
                    ExpiresUtc = System.DateTimeOffset.UtcNow.AddDays(7)
                };

                // Dùng HttpContext để ký và tạo cookie
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return LocalRedirect(returnUrl ?? "/");
            }

            ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không hợp lệ.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var registerResult = await _authService.RegisterAsync(model);

            if (registerResult.Success)
            {
                // Sử dụng TempData để gửi thông báo thành công tới trang Login
                TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Bây giờ bạn có thể đăng nhập.";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                // Nếu đăng ký thất bại, hiển thị lỗi
                ModelState.AddModelError(string.Empty, registerResult.Message ?? "Đã có lỗi xảy ra trong quá trình đăng ký.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult VerifyEmail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}