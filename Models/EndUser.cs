namespace RizeUp.Models
{
    public class EndUser : Person
    {
        public List<Resume> Resumes { get; set; } = new();
        public List<Portfolio> Portfolios { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();

    }
}
