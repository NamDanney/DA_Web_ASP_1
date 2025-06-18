using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DA_Web.Models
{
    [Table("ChatMessages")]
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        [Required]
        public string Message { get; set; }
        public bool IsBot { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}