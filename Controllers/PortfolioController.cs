using Microsoft.AspNetCore.Mvc;
using RizeUp.DTOs;

namespace RizeUp.Controllers
{
    public class PortfolioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult NewPortfolio()
        {
            return View(new PortfolioDTO());
        }
        public IActionResult CreatePortfolio()
        {
            return View(new PortfolioDTO());
        }

    }
}
