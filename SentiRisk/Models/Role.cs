using System.ComponentModel.DataAnnotations;

namespace SentiRisk.Models
{
    public class Role
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; } = string.Empty;
        public virtual ICollection<User>? ListeUsers { get; set; }
    }
}
