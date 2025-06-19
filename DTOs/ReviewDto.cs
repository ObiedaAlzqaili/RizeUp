using RizeUp.Models;

namespace RizeUp.DTOs
{
    public class ReviewDto
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
