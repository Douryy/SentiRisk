namespace SentiRiskWeb.Models.Dtos
{
    // ===== DTO final utilisé par le Dashboard =====
    public class PortfolioDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";

        public List<AssetDto> Assets { get; set; } = new();
    }

    public class AssetDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Ticker { get; set; } = "";
        public string Sector { get; set; } = "";
        public decimal CurrentPrice { get; set; }

        public decimal Weight { get; set; } // depuis PortfolioAsset
    }
}

