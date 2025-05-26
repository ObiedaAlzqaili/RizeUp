using System.ComponentModel.DataAnnotations;

namespace RizeUp.Models
{
    public class Language
    {
        [Key] public int Id { get; set; }
        public string LanguageName { get; set; }
        public string Level { get; set; }

        public int ResumeId { get; set; }
        public Resume Resume { get; set; }
    }
}
