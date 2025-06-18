using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DA_Web.Models
{
    public class Hotel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        public string Address { get; set; }
        [Column(TypeName = "decimal(10, 6)")]
        public decimal? Latitude { get; set; }
        [Column(TypeName = "decimal(10, 6)")]
        public decimal? Longitude { get; set; }

        public virtual ICollection<LocationHotel> LocationHotels { get; set; }
    }
}