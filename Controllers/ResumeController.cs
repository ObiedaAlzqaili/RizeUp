using Microsoft.AspNetCore.Mvc;
using RizeUp.Repository;

namespace RizeUp.Controllers
{
    public class ResumeController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult NewResume()
        {
            return View();
        }
    }
}
