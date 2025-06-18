using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DA_Web.Models
{
    public class ScheduleActivity
    {
        [Key]
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        [Required]
        public string Activity { get; set; }

        [ForeignKey("ScheduleId")]
        public virtual TourSchedule TourSchedule { get; set; }
    }
}