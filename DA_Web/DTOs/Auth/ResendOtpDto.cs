using System.ComponentModel.DataAnnotations;

namespace DA_Web.DTOs.Auth
{
    public class ResendOtpDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}