using System.Collections.Generic;

namespace RizeUp.DTOs
{
    public class PortfolioJsonDto
    {
        // from PersonalInfo
        public string? Title { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string? Address { get; set; }
        public string Summery { get; set; } = "";
        public string? ImageBase64 { get; set; }
        public string? ImageFileName { get; set; }
        public string? ImageContentType { get; set; }
        public string? GitHubLink { get; set; }
        public string? LinkedinLink { get; set; }

        // Child lists
        public List<ServiceItem>? Services { get; set; }
        public List<ProjectItem1>? Projects { get; set; }
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
        public string? ImageBase64 { get; set; }
        public string? ImageFileName { get; set; }
        public string? ImageContentType { get; set; }
        public bool? IsOngoing { get; set; }
        public string? ProjectLink { get; set; }
    }
}