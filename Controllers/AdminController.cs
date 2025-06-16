using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RizeUp.Models;
using RizeUp.Repository;

namespace RizeUp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IPortfolioRepo _portfolioRepo;
        private readonly IResumeRepo _resumeRepo;
        private readonly UserManager<Person> _userManager;

        public AdminController(
            IPortfolioRepo portfolioRepo,
            IResumeRepo resumeRepo,
            UserManager<Person> userManager)
        {
            _portfolioRepo = portfolioRepo;
            _resumeRepo = resumeRepo;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardViewModel
            {
                TotalResumes = await _resumeRepo.GetCountAsync(),
                TotalPortfolios = await _portfolioRepo.GetCountAsync(),
                TotalUsers = await _userManager.Users.CountAsync(),
                RecentUsers = await _userManager.GetRecentUsersAsync(5),
                RecentActivity = await _resumeRepo.GetRecentActivityAsync(7),
                //UserReviews = await _portfolioRepo.GetRecentReviewsAsync(3)
            };

            return View(model);
        }
    }
}