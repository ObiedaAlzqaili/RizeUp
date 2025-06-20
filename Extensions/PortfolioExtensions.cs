using RizeUp.DTOs;
using RizeUp.Models;

namespace RizeUp.Extensions
{
    public class PortfolioExtensions
    {

        public static Portfolio MapToPortfolioEntity(PortfolioDto dto, string userId)
        {
            return new Portfolio
            {
                // Top-level properties:
                Id = dto.Id,
                PortfolioTemplateId = dto.PortfolioTemplateId, // Assuming this is set in the DTO
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Title = dto.Title,
                Address = dto.Address,
                Summery = dto.Summery,
                ImageBase64 = dto.ImageBase64,
                ImageContentType = dto.ImageContentType,
                ImageFileName = dto.ImageFileName,
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
                    ImageBase64 = p.ImageBase64,
                    ImageContentType = p.ImageContentType,
                    ImageFileName = p.ImageFileName,
                    ProjectLink = p.ProjectLink
                }).ToList() ?? new List<Project>(),
                Skills = dto.Skills?.Select(s => new Skill
                {
                    SkillName = s.SkillName,
                    SkillType = s.SkillType
                }).ToList() ?? new List<Skill>(),
            };
        }

        public static List<PortfolioDto> MapToPortfolioJsonDtoList(List<Portfolio> portfolios)
        {
            if (portfolios == null || portfolios.Count == 0)
                return new List<PortfolioDto>();

            return portfolios.Select(p => new PortfolioDto
            {
                Id = p.Id,
                Title = p.Title,
                PortfolioTemplateId = p.PortfolioTemplateId,
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
        public static PortfolioDto MapToPortfolioDto(Portfolio portfolio)
        {
            if (portfolio == null)
                return null;

            return new PortfolioDto
            {
                Id = portfolio.Id,
                Title = portfolio.Title,
                PortfolioTemplateId = portfolio.PortfolioTemplateId,
                FirstName = portfolio.FirstName,
                LastName = portfolio.LastName,
                Email = portfolio.Email,
                PhoneNumber = portfolio.PhoneNumber,
                Address = portfolio.Address,
                Summery = portfolio.Summery,
                GitHubLink = portfolio.GitHubLink,
                LinkedinLink = portfolio.LinkedinLink,
                ImageBase64 = portfolio.ImageBase64,
                ImageFileName = portfolio.ImageFileName,
                ImageContentType = portfolio.ImageContentType,
                CreatedDate = portfolio.CreatedDate,
                ModifiedDate = portfolio.ModifiedDate,
                Services = portfolio.Services?.Select(s => new ServiceItem
                {
                    ServiceName = s.ServiceName,
                    ServiceDescription = s.ServiceDescription
                }).ToList() ?? new List<ServiceItem>(),
                Projects = portfolio.Projects?.Select(p => new ProjectItem1
                {
                    ProjectName = p.ProjectName,
                    ProjectDescription = p.ProjectDescription,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    IsOngoing = p.IsOngoing,
                    ProjectLink = p.ProjectLink,
                    ImageBase64 = p.ImageBase64,
                    ImageFileName = p.ImageFileName,
                    ImageContentType = p.ImageContentType
                }).ToList() ?? new List<ProjectItem1>(),
                Skills = portfolio.Skills?.Select(s => new SkillItem
                {
                    SkillName = s.SkillName,
                    SkillType = s.SkillType,
                }).ToList() ?? new List<SkillItem>(),
            };
        }


    }
}
