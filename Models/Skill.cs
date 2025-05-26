using System.ComponentModel.DataAnnotations;

namespace RizeUp.Models
{
    public class Skill
    {
        [Key] public int Id { get; set; }
        public string SkillName { get; set; }
        public string SkillType { get; set; }

        public int ResumeId { get; set; }
        public Resume Resume { get; set; }
    }
}
