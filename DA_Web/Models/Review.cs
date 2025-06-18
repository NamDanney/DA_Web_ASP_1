using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DA_Web.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TourId { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("TourId")]
        public virtual Tour Tour { get; set; }

        public virtual ICollection<ReviewImage> ReviewImages { get; set; }
    }
}