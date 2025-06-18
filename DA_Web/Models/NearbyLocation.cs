namespace DA_Web.Models
{
    public class NearbyLocation
    {
        public int LocationId { get; set; }
        public virtual Location Location { get; set; }

        public int NearbyLocationId { get; set; }
        public virtual Location Nearby { get; set; }
    }
}