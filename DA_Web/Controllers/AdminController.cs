using DA_Web.Models.Enums;
using DA_Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DA_Web.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;

        // Tiêm IUserService vào
        public AdminController(IUserService userService)
        {
            _userService = userService;
        }

        // Action cho trang Dashboard
        public IActionResult Index()
        {
            return View();
        }

        // Action mới cho trang Quản lý người dùng
        [HttpGet]
        public async Task<IActionResult> UserManagement()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        // [GET] /Admin/EditUser/5
        [HttpGet]
        public async Task<IActionResult> EditUser(int id)
        {
            // Chúng ta có thể dùng lại phương thức đã có để lấy user
            var user = await _userService.GetUserByIdForClaimsAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // [POST] /Admin/EditUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(int id, [FromForm] RoleType role)
        {
            var result = await _userService.UpdateUserRoleAsync(id, role);

            // Optional: Thêm TempData để hiển thị thông báo thành công
            if (result.Success)
            {
                TempData["SuccessMessage"] = $"Cập nhật vai trò cho người dùng ID {id} thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction("UserManagement");
        }

        //Xóa người dùng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            // Ngăn admin tự xóa chính mình
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(currentUserId) && int.Parse(currentUserId) == id)
            {
                TempData["ErrorMessage"] = "Bạn không thể xóa chính mình.";
                return RedirectToAction("UserManagement");
            }

            var result = await _userService.DeleteUserAsync(id);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                // Thêm trường hợp nếu người dùng này có dữ liệu liên quan (tour, review...) mà DB không cho xóa
                TempData["ErrorMessage"] = result.Message ?? "Có lỗi xảy ra, có thể do người dùng này còn dữ liệu liên quan.";
            }

            return RedirectToAction("UserManagement");
        }
    }
}