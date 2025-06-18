using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DA_Web.Models
{
    public class Location
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; }

        public string Description { get; set; }

        [Column(TypeName = "decimal(10, 6)")]
        public decimal Latitude { get; set; }

        [Column(TypeName = "decimal(10, 6)")]
        public decimal Longitude { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual LocationDetail LocationDetail { get; set; }
        public virtual TravelInfo TravelInfo { get; set; }
        public virtual ICollection<BestTime> BestTimes { get; set; }
        public virtual ICollection<TravelMethod> TravelMethods { get; set; }
        public virtual ICollection<Experience> Experiences { get; set; }
        public virtual ICollection<Cuisine> Cuisines { get; set; }
        public virtual ICollection<Tip> Tips { get; set; }
        public virtual ICollection<LocationImage> LocationImages { get; set; }
        public virtual ICollection<LocationHotel> LocationHotels { get; set; }
        public virtual ICollection<NearbyLocation> NearbyLocations { get; set; }
        public virtual ICollection<NearbyLocation> LocationsNearby { get; set; }
        public virtual ICollection<TourLocation> TourLocations { get; set; }
    }
}