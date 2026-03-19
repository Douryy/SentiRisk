
using Microsoft.AspNetCore.Mvc;
using SentiRiskWeb.Services;
using SentiRiskWeb.ViewModels;

namespace SentiRiskWeb.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApiClientService _api;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(ApiClientService api, ILogger<DashboardController> logger)
        {
            _api = api;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var portfolios = await _api.GetPortfoliosAsync();
                var scenarios = await _api.GetScenariosAsync();

                var vm = new DashboardViewModel
                {
                    Portfolios = portfolios,
                    Scenarios = scenarios
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du chargement du dashboard");
                return View(new DashboardViewModel());
            }
        }
    }
}