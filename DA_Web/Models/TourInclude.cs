using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DA_Web.Models
{
    public class TourInclude
    {
        [Key] public int Id { get; set; }
        public int TourId { get; set; }
        [Required] public string Description { get; set; }
        [ForeignKey("TourId")] public virtual Tour Tour { get; set; }
    }
}