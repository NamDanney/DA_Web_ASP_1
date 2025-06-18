using DA_Web.Data;
using DA_Web.DTOs.Auth;
using DA_Web.DTOs.Common;
using DA_Web.Models;
using DA_Web.Services.Interfaces;
using DA_Web.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DA_Web.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        public UserService(ApplicationDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<UserInfoDto?> GetUserProfile(ClaimsPrincipal userClaims)
        {
            var userId = userClaims.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return null;

            var dbUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == int.Parse(userId));
            if (dbUser == null) return null;

            return new UserInfoDto
            {
                Id = dbUser.Id,
                Username = dbUser.Username,
                Email = dbUser.Email,
                FullName = dbUser.FullName,
                Phone = dbUser.Phone,
                Avatar = string.IsNullOrEmpty(dbUser.Avatar) ? "/images/default-avatar.png" : "/" + dbUser.Avatar.Replace("\\", "/")
            };
        }

        public async Task<ApiResponse<bool>> UpdateUserProfileAsync(int userId, UpdateUserProfileDto model)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return new ApiResponse<bool> { Success = false, Message = "Không tìm thấy người dùng." };

            user.FullName = model.FullName;
            user.Phone = model.Phone;
            await _context.SaveChangesAsync();
            return new ApiResponse<bool> { Success = true, Data = true };
        }

        public async Task<ApiResponse<string>> UpdateUserAvatarAsync(int userId, IFormFile avatarFile)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return new ApiResponse<string> { Success = false, Message = "Không tìm thấy người dùng." };

            var savedPath = await _fileService.SaveFileAsync(avatarFile, "avatars");
            if (string.IsNullOrEmpty(savedPath))
                return new ApiResponse<string> { Success = false, Message = "Lưu file thất bại." };

            if (!string.IsNullOrEmpty(user.Avatar) && !user.Avatar.Contains("default-avatar.png"))
            {
                _fileService.DeleteFile(user.Avatar);
            }

            user.Avatar = savedPath;
            await _context.SaveChangesAsync();

            return new ApiResponse<string> { Success = true, Message = "Cập nhật ảnh đại diện thành công!", Data = savedPath };
        }

        public async Task<User?> GetUserByIdForClaimsAsync(int userId)
        {
            // Trả về toàn bộ đối tượng User để có thể lấy tất cả thông tin cần thiết
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.AsNoTracking().ToListAsync();
        }

        public async Task<ApiResponse<bool>> UpdateUserRoleAsync(int userId, RoleType newRole)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new ApiResponse<bool> { Success = false, Message = "Không tìm thấy người dùng." };
            }

            user.Role = newRole;
            await _context.SaveChangesAsync();

            return new ApiResponse<bool> { Success = true, Data = true, Message = "Cập nhật vai trò thành công." };
        }

        public async Task<ApiResponse<bool>> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new ApiResponse<bool> { Success = false, Message = "Không tìm thấy người dùng." };
            }

            // Cân nhắc: Nếu người dùng có các bài đăng hoặc tour, bạn có thể muốn xử lý chúng trước khi xóa.
            // Để đơn giản, chúng ta sẽ xóa trực tiếp.
            // Nếu người dùng có ảnh đại diện, chúng ta cũng nên xóa nó.
            if (!string.IsNullOrEmpty(user.Avatar) && !user.Avatar.Contains("default-avatar.png"))
            {
                _fileService.DeleteFile(user.Avatar);
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return new ApiResponse<bool> { Success = true, Message = "Xóa người dùng thành công." };
        }
    }
}