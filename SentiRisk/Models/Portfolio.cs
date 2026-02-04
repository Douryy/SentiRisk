using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SentiRisk.Models
{
    public class Portfolio
    {
        
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set;}

    }
}
