using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SentiRisk.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string? UserName { get; set; } = string.Empty;
        [Required]
        public string? Email { get; set;} = string.Empty;
        [Required]
        public string? PasswordHash { get; set; } = string.Empty;

        [ForeignKey("Role")]
        public int RoleId { get; set; }
        public virtual Role? Role { get; set; }
        public virtual ICollection<Portfolio>? ListePortfolio { get; set; }
    }
}
