using System.ComponentModel.DataAnnotations;

namespace RizeUp.Models
{
    public class Resume : PersonalInfo
    {
        [Key] public int ResumeId { get; set; }
        public string CreatedDate { get; set; } = DateOnly.FromDateTime(DateTime.Now).ToString();
        public string? ModifiedDate { get; set; }

        public bool IsDeleted { get; set; } = false;

        public List<Education> Educations { get; set; } =   new List<Education>();
        public List<Experience> Experiences { get; set; } = new List<Experience> ();
        public List<Project> Projects { get; set; } = new List<Project>(); 
        public List<Skill> Skills { get; set; } = new List<Skill> ();
        public List<Language> Languages { get; set; } = new List<Language>();
        public List<Certificate> Certificates { get; set; } = new List<Certificate> ();

        public string EndUserId { get; set; }
        public EndUser EndUser { get; set; }
    }
}
