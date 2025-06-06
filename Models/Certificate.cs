using System.ComponentModel.DataAnnotations;

namespace RizeUp.Models
{
    public class Certificate
    {
        [Key] public int Id { get; set; }

        public string? Title { get; set; }
        public string? ProviderName { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? Field { get; set; }
        public double? GPA { get; set; }

        public int ResumeId { get; set; }
        public Resume Resume { get; set; }
    }
}
