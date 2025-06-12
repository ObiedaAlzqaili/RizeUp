using RizeUp.DTOs;
using RizeUp.Models;

namespace RizeUp.Extensions
{
    public class PortfolioExtensions
    {
        private Portfolio MapToPortfolioEntity(PortfolioDto dto, string userId)
        {
            return new Portfolio
            {
                // Top-level properties:
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Title = dto.Title,
                Address = dto.Address,
                Summery = dto.Summery,
                GitHubLink = dto.GitHubLink,
                LinkedinLink = dto.LinkedinLink,
                EndUserId = userId,

                // Timestamps
                CreatedDate = DateOnly.FromDateTime(DateTime.UtcNow).ToString(),
                ModifiedDate = DateTime.UtcNow.ToString(),
                IsDeleted = false,

                // Services
                Services = dto.Services?.Select(s => new Service
                {
                    ServiceName = s.ServiceName,
                    ServiceDescription = s.ServiceDescription
                }).ToList() ?? new List<Service>(),

                // Projects
                Projects = dto.Projects?.Select(p => new Project
                {
                    ProjectName = p.ProjectName,
                    ProjectDescription = p.ProjectDescription,
                    StartDate = string.IsNullOrWhiteSpace(p.StartDate) ? null : p.StartDate,
                    EndDate = string.IsNullOrWhiteSpace(p.EndDate) ? null : p.EndDate,
                    IsOngoing = p.IsOngoing,
                    ProjectLink = p.ProjectLink
                }).ToList() ?? new List<Project>()
            };
        }

        public List<PortfolioDto> MapToPortfolioJsonDtoList(List<Portfolio> portfolios)
        {
            if (portfolios == null || portfolios.Count == 0)
                return new List<PortfolioDto>();

            return portfolios.Select(p => new PortfolioDto
            {
                Title = p.Title,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                PhoneNumber = p.PhoneNumber,
                Address = p.Address,
                Summery = p.Summery,
                GitHubLink = p.GitHubLink,
                LinkedinLink = p.LinkedinLink,

                Services = p.Services?.Select(s => new ServiceItem
                {
                    ServiceName = s.ServiceName,
                    ServiceDescription = s.ServiceDescription
                }).ToList() ?? new List<ServiceItem>(),

                Projects = p.Projects?.Select(proj => new ProjectItem1
                {
                    ProjectName = proj.ProjectName,
                    ProjectDescription = proj.ProjectDescription,
                    StartDate = proj.StartDate,
                    EndDate = proj.EndDate,

                    ProjectLink = proj.ProjectLink
                }).ToList() ?? new List<ProjectItem1>()
            }).ToList();
        }
    }
}
