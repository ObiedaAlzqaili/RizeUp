using RizeUp.DTOs;

namespace RizeUp.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalResumes { get; set; }
        public int TotalPortfolios { get; set; }
        public int TotalUsers { get; set; }
        public List<Person?> RecentUsers { get; set; }
        public List<Review> Reviews { get; set; }
        public List<Resume> Recent { get; set; }
        public List<Portfolio> UserReviews { get; set; }
    }
   
    public class PortfolioReview
    {
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}
