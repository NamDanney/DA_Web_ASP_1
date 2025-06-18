
using System.ComponentModel.DataAnnotations;

namespace DA_Web.DTOs.Auth
{
    public class VerifyOtpDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(6)]
        public string Otp { get; set; }
    }
}