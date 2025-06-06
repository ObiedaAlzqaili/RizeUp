using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RizeUp.DTOs
{
    public class CreatePortfolioDto
    {
        // === Step 1 fields ===
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }
        [Url(ErrorMessage = "Please enter a valid LinkedIn URL.")]
        public string? LinkedIn { get; set; }
        [Url(ErrorMessage = "Please enter a valid GitHub URL.")]
        public string? GitHub { get; set; }
        public IFormFile? ProfileImage { get; set; }
        // Holds the base64 string of the uploaded profile image between steps
        public string? ProfileImageBase64 { get; set; }
        [Required(ErrorMessage = "Summary/About Me is required.")]
        public string Summary { get; set; }
        // === Step 2 fields ===
        [Required(ErrorMessage = "Services are required.")]
        public string Services { get; set; }
        public List<ProjectItemDto> Projects { get; set; } = new();
        [Required(ErrorMessage = "Skills are required.")]
        public string Skills { get; set; }
        // Tracks which step (1–3) is currently active
        public int CurrentStep { get; set; }
    }
    public class ProjectItemDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile? Image { get; set; }
        // Holds the base64 string of the uploaded project image between steps
        public string? ImageBase64 { get; set; }
    }
}
