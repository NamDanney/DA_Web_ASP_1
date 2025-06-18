using DA_Web.Models.Enums;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DA_Web.Models
{
    public class Tour
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Destination { get; set; }
        [Required]
        [StringLength(255)]
        public string Image { get; set; }
        [Required]
        [StringLength(100)]
        public string DepartureFrom { get; set; }
        [Required]
        [StringLength(50)]
        public string Duration { get; set; }
        [Required]
        public string Description { get; set; }
        public TourStatus Status { get; set; } = TourStatus.pending;
        public int? UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public virtual ICollection<TourHighlight> TourHighlights { get; set; }
        public virtual ICollection<TourSchedule> TourSchedules { get; set; }
        public virtual ICollection<TourInclude> TourIncludes { get; set; }
        public virtual ICollection<TourExclude> TourExcludes { get; set; }
        public virtual ICollection<TourNote> TourNotes { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<TourLocation> TourLocations { get; set; }
    }
}