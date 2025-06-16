using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace RizeUp.Models
{
    public class Portfolio : PersonalInfo
    {
        [Key] public int Id { get; set; }

        public List<Service>? Services { get; set; } 
        public List<Project>? Projects { get; set; }
        public List<Skill>? Skills { get; set; }
        public bool IsDeleted { get; set; } = false;

        public int PortfolioTemplateId { get; set; }
        public string? ImageBase64 { get; set; }
        public string? ImageFileName { get; set; }
        public string? ImageContentType { get; set; }
        public string EndUserId { get; set; }
        public EndUser EndUser { get; set; }

        public string CreatedDate { get; set; } = DateOnly.FromDateTime(DateTime.Now).ToString();
        public string? ModifiedDate { get; set; }
        
    }

}

