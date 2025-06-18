using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DA_Web.Models
{
    public class TourSchedule
    {
        [Key]
        public int Id { get; set; }
        public int TourId { get; set; }
        [Required]
        [StringLength(50)]
        public string Day { get; set; }
        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [ForeignKey("TourId")]
        public virtual Tour Tour { get; set; }

        public virtual ICollection<ScheduleActivity> ScheduleActivities { get; set; }
        public virtual ICollection<TourLocation> TourLocations { get; set; }
    }
}