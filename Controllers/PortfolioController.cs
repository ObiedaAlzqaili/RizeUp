using Microsoft.AspNetCore.Mvc;

namespace RizeUp.Controllers
{
    public class PortfolioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
