using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RizeUp.Models;
using RizeUp.Repositories;
using RizeUp.Repository;

namespace RizeUp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IPortfolioRepo _portfolioRepo;
        private readonly IResumeRepo _resumeRepo;
        private readonly UserManager<Person> _userManager;
        private readonly IReviewRepo _reviewRepo;

        public AdminController(
            IPortfolioRepo portfolioRepo,
            IResumeRepo resumeRepo,
            UserManager<Person> userManager,
            IReviewRepo reviewRepo)
        {
            _portfolioRepo = portfolioRepo;
            _resumeRepo = resumeRepo;
            _userManager = userManager;
            _reviewRepo = reviewRepo;
        }

        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardViewModel
            {
                TotalResumes = await _resumeRepo.GetCountAsync(),
                Reviews = await _reviewRepo.GetRecentAsync(5),
                TotalUsers = await _userManager.Users.CountAsync(),
                RecentUsers = await _userManager.GetLastFiveEndUsersAsync(),
                //RecentActivity = await _resumeRepo.GetRecentActivityAsync(7),
                //UserReviews = await _portfolioRepo.GetRecentReviewsAsync(3)
            };

            return View(model);
        }
            public async Task<IActionResult> Users()
            {
                var model = new AdminDashboardViewModel
                {
                    RecentUsers = await _userManager.Users
                        .OrderByDescending(u => u.Id)
                        .Take(15)
                        .Cast<Person>()
                        .ToListAsync(),
                };

                return View(model);
            }
            public async Task<IActionResult> Resumes()
            {
                var model = new AdminDashboardViewModel
                {
                    Recent = await _resumeRepo.GetPortfolioCount(15),
                };

                return View(model);
            }
            public async Task<IActionResult> Portfolios()
            {
                var model = new AdminDashboardViewModel
                {
                    TotalResumes = await _resumeRepo.GetCountAsync(),
                    Reviews = await _reviewRepo.GetRecentAsync(5),
                    TotalUsers = await _userManager.Users.CountAsync(),
                    UserReviews = await _portfolioRepo.GetPortfoliosCount(15),
                    RecentUsers = await _userManager.GetLastFiveEndUsersAsync(),
                };

                return View(model);
            }
            public async Task<IActionResult> Reviews()
            {
                var model = new AdminDashboardViewModel
                {
                    TotalResumes = await _resumeRepo.GetCountAsync(),
                    Reviews = await _reviewRepo.GetRecentAsync(15),
                    TotalUsers = await _userManager.Users.CountAsync(),
                    RecentUsers = await _userManager.GetLastFiveEndUsersAsync(),
                };

                return View(model);
            }
    
    }
}