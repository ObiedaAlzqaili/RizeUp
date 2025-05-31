using System.ComponentModel.DataAnnotations;

namespace RizeUp.Models
{
    public class Experience
    {
        [Key] public int Id { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public DateOnly StartDate { get; set; } 
        public DateOnly? EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public string Duties { get; set; }

        public int ResumeId { get; set; }
        public Resume Resume { get; set; }

        


    }
}
