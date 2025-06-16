using System.ComponentModel.DataAnnotations;

namespace RizeUp.Models
{
    public class Resume : PersonalInfo
    {
        [Key] public int ResumeId { get; set; }
        public string CreatedDate { get; set; } 
        public string? ModifiedDate { get; set; }

        public bool IsDeleted { get; set; } = false;

        public List<Education> Educations { get; set; }
        public List<Experience> Experiences { get; set; }
        public List<Project> Projects { get; set; }
        public List<Skill> Skills { get; set; } 
        public List<Language> Languages { get; set; } 
        public List<Certificate> Certificates { get; set; }

        public int? ResumeTemplateId { get; set; }
        public string EndUserId { get; set; }
        public EndUser EndUser { get; set; }
    }
}
