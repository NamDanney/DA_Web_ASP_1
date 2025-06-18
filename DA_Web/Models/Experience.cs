using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DA_Web.Models
{
    public class Experience
    {
        [Key]
        public int Id { get; set; }
        public int LocationId { get; set; }
        [Required]
        public string Description { get; set; }

        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }
    }
}