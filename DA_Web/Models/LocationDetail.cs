using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DA_Web.Models
{
    public class LocationDetail
    {
        [Key]
        public int Id { get; set; }

        public int LocationId { get; set; }

        public string? Subtitle { get; set; }
        public string? Introduction { get; set; }

        [StringLength(255)]
        public string? WhyVisitArchitectureTitle { get; set; }
        public string? WhyVisitArchitectureText { get; set; }
        public string? WhyVisitCulture { get; set; }

        // Navigation Property for the one-to-one relationship
        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }
    }
}