using System.ComponentModel.DataAnnotations;

namespace RizeUp.Models
{
    public class PersonalInfo
    {
        public string? Title { get; set; }
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        [EmailAddress] public string Email { get; set; }
        [Phone] public string PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string Summery { get; set; }
        

        public string? GitHubLink { get; set; }
        public string? LinkedinLink { get; set; }
                
    }
}
