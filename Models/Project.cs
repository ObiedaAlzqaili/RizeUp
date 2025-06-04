using System.ComponentModel.DataAnnotations;

namespace RizeUp.Models
{
    public class Project
    {
        [Key] public int Id { get; set; }
        public string? ProjectName { get; set; }
        public string? ProjectDescription { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        
        public string? ProjectLink { get; set; }

        public int ResumeId { get; set; }
        public Resume Resume { get; set; }

        public int PortfolioId { get; set; }
        public Portfolio Portfolio { get; set; }
    }
}
