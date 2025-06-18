using DA_Web.Models.Enums;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.ComponentModel.DataAnnotations;

namespace DA_Web.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(15)]
        public string Phone { get; set; }

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        public RoleType Role { get; set; } = RoleType.user;

        [Required]
        [StringLength(255)]
        public string FullName { get; set; }

        [StringLength(255)]
        public string? Avatar { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // THÊM PROPERTY NÀY
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual ICollection<Tour> Tours { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}