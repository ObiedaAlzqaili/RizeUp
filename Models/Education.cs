using System.ComponentModel.DataAnnotations;

namespace RizeUp.Models
{
    public class Education
    {
        [Key] public int Id { get; set; }
        public string? CollegeName { get; set; }
        public string? DegreeField { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? Major { get; set; }
        public double? GPA { get; set; }

        public int ResumeId { get; set; }
        public Resume Resume { get; set; }
    }
}
