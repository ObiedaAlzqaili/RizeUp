using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RizeUp.DTOs;
using RizeUp.Models;
using RizeUp.Repositories;

namespace RizeUp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IReviewRepo _reviews;
        public HomeController(ILogger<HomeController> logger,IReviewRepo reviews)
        {
            _logger = logger;
            _reviews = reviews;
        }
      
        public IActionResult Index()
        {
            var recentReviews = _reviews.GetRecentAsync(5).Result;
            ViewBag.RecentReviews = recentReviews;
            return View(new ReviewDto());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SubmitReview(ReviewDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", dto);
            }else
            {
                var review = new Review
                {
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    UserName = User.Identity.Name,
                    Rating = dto.Rating,
                    Comment = dto.Comment,
                    CreatedAt = DateTime.UtcNow
                };
                await _reviews.AddAsync(review);
                TempData["SuccessMessage"] = "Review submitted successfully!";
                return RedirectToAction("Index");
            }
           
        }

        
    }
}
