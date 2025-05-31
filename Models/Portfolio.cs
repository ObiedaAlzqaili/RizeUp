using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace RizeUp.Models
{
    public class Portfolio : PersonalInfo
    {
        [Key] public int Id { get; set; }
        public string PersonalImage { get; set; }

        public List<Service> Services { get; set; } 
        public List<Project> Projects { get; set; }
        public bool IsDeleted { get; set; } = false;

        public string EndUserId { get; set; }
        public EndUser EndUser { get; set; }

        public DateOnly CreatedDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public DateOnly? ModifiedDate { get; set; }
    }
}

