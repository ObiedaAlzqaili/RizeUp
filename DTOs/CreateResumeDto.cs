using System.ComponentModel.DataAnnotations;

namespace RizeUp.DTOs
{
    public class CreateResumeDto
    {
        [Required]
        [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        //[Url(ErrorMessage = "Invalid LinkedIn URL.")]
        public string? LinkedinLink { get; set; }

        //[Url(ErrorMessage = "Invalid GitHub URL.")]
        public string? GitHubLink { get; set; }

        [Required(ErrorMessage = "Summary/About Me is required.")]
        [StringLength(1000, ErrorMessage = "Summary cannot exceed 1000 characters.")]
        public string Summary { get; set; }

        [Required(ErrorMessage = "Education is required.")]
        [StringLength(2000, ErrorMessage = "Education details cannot exceed 2000 characters.")]
        public string Education { get; set; }

        [Required(ErrorMessage = "Experience is required.")]
        [StringLength(2000, ErrorMessage = "Experience details cannot exceed 2000 characters.")]
        public string Experience { get; set; }

        [Required(ErrorMessage = "Skills are required.")]
        [StringLength(1000, ErrorMessage = "Skills list cannot exceed 1000 characters.")]
        public string Skills { get; set; }

        public int ResumedTemplateId { get; set; } = 1;

        public int CurrentStep { get; set; } = 1;
    }

}
