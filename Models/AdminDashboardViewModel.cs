namespace RizeUp.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalResumes { get; set; }
        public int TotalPortfolios { get; set; }
        public int TotalUsers { get; set; }
        public List<Person> RecentUsers { get; set; }
        public List<ResumeActivity> RecentActivity { get; set; }
        public List<PortfolioReview> UserReviews { get; set; }
    }
    public class ResumeActivity
    {
        public string UserName { get; set; }
        public string ActivityType { get; set; }
        public DateTime ActivityDate { get; set; }
        public string Status { get; set; }
    }
    public class PortfolioReview
    {
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}
