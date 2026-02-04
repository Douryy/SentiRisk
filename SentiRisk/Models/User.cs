using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SentiRisk.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Email { get; set;}
        [Required]
        public string? PasswordHash { get; set; }

        [ForeignKey("Role")]
        public int RoleId { get; set; }
        public virtual ICollection<Portfolio>? ListePortfolio { get; set; }
    }
}
