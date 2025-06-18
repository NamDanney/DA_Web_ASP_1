using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DA_Web.Models
{
    public class TourExclude
    {
        [Key] public int Id { get; set; }
        public int TourId { get; set; }
        [Required] public string Description { get; set; }
        [ForeignKey("TourId")] public virtual Tour Tour { get; set; }
    }
}