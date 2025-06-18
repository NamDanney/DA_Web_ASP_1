namespace DA_Web.Models
{
    public class TourLocation
    {
        public int TourId { get; set; }
        public virtual Tour Tour { get; set; }

        public int LocationId { get; set; }
        public virtual Location Location { get; set; }

        public int ScheduleId { get; set; }
        public virtual TourSchedule TourSchedule { get; set; }
    }
}