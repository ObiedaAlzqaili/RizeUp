using System.ComponentModel.DataAnnotations;

namespace RizeUp.Models
{
    public class PersonalInfo
    {
        [Required] public string FirstName { get; set; }
        public string? SecondName { get; set; }
        [Required] public string LastName { get; set; }
        [EmailAddress] public string Email { get; set; }
        [Phone] public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Bio { get; set; }
        public string Title { get; set; }

        public string? GitHubLink { get; set; }
        public string? LinkedinLink { get; set; }
        public string? InstagramLink { get; set; }
        public string? FacebookLink { get; set; }
        

        
    }
}
