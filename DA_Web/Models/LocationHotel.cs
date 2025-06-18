using System.ComponentModel.DataAnnotations.Schema;

namespace DA_Web.Models
{
    public class LocationHotel
    {
        public int LocationId { get; set; }
        public virtual Location Location { get; set; }

        public int HotelId { get; set; }
        public virtual Hotel Hotel { get; set; }
    }
}