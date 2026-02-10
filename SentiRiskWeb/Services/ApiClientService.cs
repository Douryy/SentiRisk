
using System.Text.Json;
using SentiRiskWeb.Models.Dtos;

namespace SentiRiskWeb.Services
{
    public class ApiClientService
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions _jsonOptions =
            new() { PropertyNameCaseInsensitive = true };

        public ApiClientService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<PortfolioDto>> GetPortfoliosAsync()
        {
            var response = await _http.GetAsync("api/portfolios");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var apiPortfolios = JsonSerializer.Deserialize<List<ApiPortfolioDto>>(json, _jsonOptions)
                               ?? new List<ApiPortfolioDto>();

            return apiPortfolios.Select(MapToPortfolio).ToList();
        }

        // ===== Mapping API → Web =====
        private static PortfolioDto MapToPortfolio(ApiPortfolioDto api)
        {
            var portfolio = new PortfolioDto
            {
                Id = api.Id,
                Name = api.Name,
                Description = api.Description
            };

            if (api.PortfolioAssets == null) return portfolio;

            foreach (var pa in api.PortfolioAssets)
            {
                if (pa.Asset == null) continue;

                portfolio.Assets.Add(new AssetDto
                {
                    Id = pa.Asset.Id,
                    Name = pa.Asset.Name,
                    Ticker = pa.Asset.Ticker,
                    Sector = pa.Asset.Sector,
                    CurrentPrice = pa.Asset.CurrentPrice,
                    Weight = pa.Weight
                });
            }

            return portfolio;
        }
    }
}
