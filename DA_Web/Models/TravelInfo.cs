using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DA_Web.Models
{
    public class TravelInfo
    {
        [Key]
        public int Id { get; set; }
        public int LocationId { get; set; }
        public string? TicketPrice { get; set; }
        public string? Tip { get; set; }

        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }
    }
}