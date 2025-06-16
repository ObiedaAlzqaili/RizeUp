using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RizeUp.Models;

namespace RizeUp.DTOs
{
    public class PortfolioDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public int PortfolioTemplateId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; } 
        public string Email { get; set; } 
        public string PhoneNumber { get; set; } 
        public string? Address { get; set; }
        public string? Summery { get; set; }
        public IFormFile? PersonalImage { get; set; }
        public string? ImageBase64 { get; set; }
        public string? ImageFileName { get; set; }
        public string? ImageContentType { get; set; }
        public string? GitHubLink { get; set; }
        public string? LinkedinLink { get; set; }
        public string? CreatedDate { get; set; }
        public string? ModifiedDate { get; set; }

        // Child lists
        public List<ServiceItem>? Services { get; set; }
        public List<ProjectItem1>? Projects { get; set; }
        public List<SkillItem>? Skills { get; set; }
    }

    public class ServiceItem
    {
        public string? ServiceName { get; set; }
        public string? ServiceDescription { get; set; }
    }

    public class ProjectItem1
    {
        public string? ProjectName { get; set; }
        public string? ProjectDescription { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public IFormFile? ProjectImage { get; set; }
        public string? ImageBase64 { get; set; }
        public string? ImageFileName { get; set; }
        public string? ImageContentType { get; set; }
        public bool? IsOngoing { get; set; }
        public string? ProjectLink { get; set; }
    }
    public class SkillItem
    {
        public string? SkillName { get; set; }
        public string? SkillType { get; set; }
        
    }
}