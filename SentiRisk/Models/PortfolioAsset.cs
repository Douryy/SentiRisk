using System.ComponentModel.DataAnnotations.Schema;

namespace SentiRisk.Models
{
    public class PortfolioAsset
    {
        [ForeignKey("Portfolio")]
        public int PortfolioId { get; set; }
        public virtual Portfolio? Portfolio { get; set; }
        [ForeignKey("Asset")]
        public int AssetId { get; set; }
       
        public virtual Asset? Asset { get; set; }
        public decimal Weight { get; set; }

    }
}
