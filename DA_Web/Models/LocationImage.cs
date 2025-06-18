using DA_Web.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DA_Web.Models
{
    [Table("Location_Images")]
    public class LocationImage
    {
        [Key]
        public int Id { get; set; }

        public int LocationId { get; set; }

        [Required]
        [StringLength(255)]
        public string ImageUrl { get; set; }

        public LocationImageType ImageType { get; set; }
        public int? ReferenceId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }
    }
}