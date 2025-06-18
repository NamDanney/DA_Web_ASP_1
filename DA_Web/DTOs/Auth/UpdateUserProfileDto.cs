using System.ComponentModel.DataAnnotations;

namespace DA_Web.DTOs.Auth
{
    public class UpdateUserProfileDto
    {
        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự")]
        public string FullName { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? Phone { get; set; }
    }
}