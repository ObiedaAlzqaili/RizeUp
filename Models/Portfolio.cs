using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace RizeUp.Models
{
    public class Portfolio : PersonalInfo
    {
        [Key] public int Id { get; set; }
        public string PersonalImage { get; set; }

        public List<Service> Services { get; set; } = new();
        public List<Project> Projects { get; set; } = new();
        public List<Experience> Experiences { get; set; } = new();

        public string EndUserId { get; set; }
        public EndUser EndUser { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; }
    }
}

