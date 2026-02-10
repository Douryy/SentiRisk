using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SentiRiskWeb.ViewModels;

namespace SentiRiskWeb.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(ILogger<DashboardController> logger)
        {
            _logger = logger;
        }

        // GET: /Dashboard/Index
        public IActionResult Index()
        {
            var vm = new DashboardViewModel();
            return View(vm);
        }
    }
}