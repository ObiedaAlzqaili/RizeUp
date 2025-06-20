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




            // this is for users
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                TempData["Success"] = "User deleted.";
            else
                TempData["Error"] = "Could not delete user.";

            return RedirectToAction(nameof(Users));
        }






        public async Task<IActionResult> Resumes()
            {
                var model = new AdminDashboardViewModel
                {
                    RecentResumes = await _resumeRepo.GetResumeCount(15),
                };

                return View(model);
            }
            public async Task<IActionResult> Portfolios()
            {
                var model = new AdminDashboardViewModel
                {
                    RecentPortfolios = await _portfolioRepo.GetPortfoliosCount(15),
          
                };

                return View(model);
            }
            public async Task<IActionResult> Reviews()
            {
                var model = new AdminDashboardViewModel
                {
                    Reviews = await _reviewRepo.GetRecentAsync(15),
                };

                return View(model);
            }
    
    }
}