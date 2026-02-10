namespace SentiRiskWeb.Models.Dtos
{
    // ===== API STRUCTURE =====

    public class ApiPortfolioDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";

        public List<ApiPortfolioAssetDto>? PortfolioAssets { get; set; }
    }

    public class ApiPortfolioAssetDto
    {
        public decimal Weight { get; set; }
        public ApiAssetDto? Asset { get; set; }
    }

    public class ApiAssetDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Ticker { get; set; } = "";
        public string Sector { get; set; } = "";
        public decimal CurrentPrice { get; set; }
    }
}
