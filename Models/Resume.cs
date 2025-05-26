using System.ComponentModel.DataAnnotations;

namespace RizeUp.Models
{
    public class Resume : PersonalInfo
    {
        [Key] public int ResumeId { get; set; }
        public DateOnly CreatedDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public DateOnly? ModifiedDate { get; set; }

        public List<Education> Educations { get; set; } = new();
        public List<Experience> Experiences { get; set; } = new();
        public List<Skill> Skills { get; set; } = new();
        public List<Language> Languages { get; set; } = new();
        public List<Certificate> Certificates { get; set; } = new();

        public string EndUserId { get; set; }
        public EndUser EndUser { get; set; }
    }
}
