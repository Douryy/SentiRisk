using System.ComponentModel.DataAnnotations;

namespace SentiRisk.Models
{
    public class Role
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }    
        public virtual ICollection<User>? ListeUsers { get; set; }
    }
}
