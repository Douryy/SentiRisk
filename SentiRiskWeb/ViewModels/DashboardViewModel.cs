using SentiRiskWeb.Models.Dtos;

namespace SentiRiskWeb.ViewModels
{
    public class DashboardViewModel
    {
        public List<PortfolioDto> Portfolios { get; set; } = new();
        public List<ScenarioDto> Scenarios { get; set; } = new();
    }
}