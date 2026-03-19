using System.ComponentModel.DataAnnotations;

namespace SentiRisk.Models
{
    public class Asset
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; } = string.Empty;
        [Required]
        public string? Ticker { get; set; } = string.Empty;
        [Required]
        public string? Sector { get; set; } = string.Empty;
        [Required]
        public decimal CurrentPrice { get; set; }
        public virtual ICollection<PortfolioAsset>? ListePortfolioAssets { get; set; }
        public virtual ICollection<News>? ListeNews { get; set; }
        public decimal StressImpact { get; set; }
    }
}
