using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DA_Web.Models
{
    [Table("Review_Images")]
    public class ReviewImage
    {
        [Key]
        public int Id { get; set; }
        public int ReviewId { get; set; }
        [Required]
        [StringLength(255)]
        public string ImageUrl { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ReviewId")]
        public virtual Review Review { get; set; }
    }
}