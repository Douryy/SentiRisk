using System.ComponentModel.DataAnnotations;

namespace SentiRisk.Models
{
    public class Scenario
    {
        public int Id { get; set; }
        [Required]
        public string? ScenarioName { get; set; } = string.Empty;
        [Required]
        public string? Description { get; set; } = string.Empty;
        [Required]
        public decimal ImpactFactor { get; set; }
        [Required]
        public string? TargetSector { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public Scenario()
        {
            CreatedAt = DateTime.Now;
        }

    }
}
