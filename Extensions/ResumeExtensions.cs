using RizeUp.DTOs;
using RizeUp.Models;

namespace RizeUp.Extensions
{
    public static class ResumeExtensions
    {
        public static IEnumerable<ResumeDto> ToResumeJsonDtoList(this IEnumerable<Resume> entities)
        {
            if (entities == null)
                return Enumerable.Empty<ResumeDto>();

            return entities.Select(entity => new ResumeDto
            {
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email,
                PhoneNumber = entity.PhoneNumber,
                Address = entity.Address,
                Summary = entity.Summery,
                Title = entity.Title,
                LinkedinLink = entity.LinkedinLink,

                Educations = entity.Educations?.Select(e => new EducationItem
                {
                    CollegeName = e.CollegeName,
                    DegreeType = e.DegreeType,
                    Major = e.Major,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    GPA = e.GPA
                }).ToList(),

                Experiences = entity.Experiences?.Select(e => new ExperienceItem
                {
                    Title = e.Title,
                    Company = e.Company,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    IsCurrent = e.IsCurrent,
                    Duties = e.Duties
                }).ToList(),

                Skills = entity.Skills?.Select(s => new SkillItem1
                {
                    SkillName = s.SkillName,
                    SkillType = s.SkillType
                }).ToList(),

                Certificates = entity.Certificates?.Select(c => new CertificateItem
                {
                    ProviderName = c.ProviderName,
                    Field = c.Field,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    GPA = c.GPA
                }).ToList(),

                Projects = entity.Projects?.Select(p => new ProjectItem
                {
                    ProjectName = p.ProjectName,
                    ProjectDescription = p.ProjectDescription,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    ProjectLink = p.ProjectLink
                }).ToList(),

                Languages = entity.Languages?.Select(l => new LanguageItem
                {
                    LanguageName = l.LanguageName,
                    Level = l.Level
                }).ToList()
            }).ToList();
        }

        public static Resume MapToResumeEntity(ResumeDto dto, string userId)
        {
            return new Resume
            {
                ResumeId = dto.Id,
                ResumeTemplateId = dto.ResumedTemplateId,
                Address = dto.Address,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Title = dto.Title,
                Summery = dto.Summary ?? dto.Summary, 
                GitHubLink = dto.GitHubLink,
                LinkedinLink = dto.LinkedinLink,
                EndUserId = userId, 

                CreatedDate = DateTime.UtcNow.ToString(),
                ModifiedDate = DateTime.UtcNow.ToShortDateString(),

             
                Educations = dto.Educations?.Select(e => new Education
                {
                    CollegeName = e.CollegeName,
                    DegreeType = e.DegreeType,
                    Major = e.Major,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    GPA = e.GPA
                }).ToList() ?? new List<Education>(),

                Experiences = dto.Experiences?.Select(x => new Experience
                {
                    Title = x.Title,
                    Company = x.Company,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    IsCurrent = x.IsCurrent ?? false,
                    Duties = x.Duties
                }).ToList() ?? new List<Experience>(),

                Skills = dto.Skills?.Select(s => new Skill
                {
                    SkillName = s.SkillName,
                    SkillType = s.SkillType
                }).ToList() ?? new List<Skill>(),

                Certificates = dto.Certificates?.Select(c => new Certificate
                {
                    ProviderName = c.ProviderName,
                    Field = c.Field,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    GPA = c.GPA
                }).ToList() ?? new List<Certificate>(),

                Projects = dto.Projects?.Select(p => new Project
                {
                    ProjectName = p.ProjectName,
                    ProjectDescription = p.ProjectDescription,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    ProjectLink = p.ProjectLink
                }).ToList() ?? new List<Project>(),

                Languages = dto.Languages?.Select(l => new Language
                {
                    LanguageName = l.LanguageName,
                    Level = l.Level
                }).ToList() ?? new List<Language>()
            };
        }

        public static ResumeDto MapToResumeJsonDto(Resume entity)
        {
            return new ResumeDto
            {
                Id = entity.ResumeId,
                ResumedTemplateId = entity.ResumeTemplateId,
                GitHubLink = entity.GitHubLink,
                CreatedDate = entity.CreatedDate,
                ModifiedDate = entity.ModifiedDate,
                EndUserId = entity.EndUserId,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email,
                PhoneNumber = entity.PhoneNumber,
                Address = entity.Address,
                Summary = entity.Summery,
                Title = entity.Title,
                LinkedinLink = entity.LinkedinLink,

                // Collections
                Educations = entity.Educations?.Select(e => new EducationItem
                {
                    CollegeName = e.CollegeName,
                    DegreeType = e.DegreeType,
                    Major = e.Major,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    GPA = e.GPA
                }).ToList(),

                Experiences = entity.Experiences?.Select(e => new ExperienceItem
                {
                    Title = e.Title,
                    Company = e.Company,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    IsCurrent = e.IsCurrent,
                    Duties = e.Duties
                }).ToList(),

                Skills = entity.Skills?.Select(s => new SkillItem1
                {
                    SkillName = s.SkillName,
                    SkillType = s.SkillType
                }).ToList(),

                Certificates = entity.Certificates?.Select(c => new CertificateItem
                {
                    ProviderName = c.ProviderName,
                    Field = c.Field,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    GPA = c.GPA
                }).ToList(),

                Projects = entity.Projects?.Select(p => new ProjectItem
                {
                    ProjectName = p.ProjectName,
                    ProjectDescription = p.ProjectDescription,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    ProjectLink = p.ProjectLink
                }).ToList(),

                Languages = entity.Languages?.Select(l => new LanguageItem
                {
                    LanguageName = l.LanguageName,
                    Level = l.Level
                }).ToList()
            };
        }

        public static List<ResumeDto> ToResumeJsonDtoList(List<Resume> entities)
        {
            if (entities == null)
                return new List<ResumeDto>();

            return entities.Select(entity => new ResumeDto
            {
                Id = entity.ResumeId,
                ResumedTemplateId = entity.ResumeTemplateId,
                GitHubLink = entity.GitHubLink,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email,
                PhoneNumber = entity.PhoneNumber,
                Address = entity.Address,
                Summary = entity.Summery,
                Title = entity.Title,
                LinkedinLink = entity.LinkedinLink,

                Educations = entity.Educations?.Select(e => new EducationItem
                {
                    CollegeName = e.CollegeName,
                    DegreeType = e.DegreeType,
                    Major = e.Major,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    GPA = e.GPA
                }).ToList(),

                Experiences = entity.Experiences?.Select(e => new ExperienceItem
                {
                    Title = e.Title,
                    Company = e.Company,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    IsCurrent = e.IsCurrent,
                    Duties = e.Duties
                }).ToList(),

                Skills = entity.Skills?.Select(s => new SkillItem1
                {
                    SkillName = s.SkillName,
                    SkillType = s.SkillType
                }).ToList(),

                Certificates = entity.Certificates?.Select(c => new CertificateItem
                {
                    ProviderName = c.ProviderName,
                    Field = c.Field,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    GPA = c.GPA
                }).ToList(),

                Projects = entity.Projects?.Select(p => new ProjectItem
                {
                    ProjectName = p.ProjectName,
                    ProjectDescription = p.ProjectDescription,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    ProjectLink = p.ProjectLink
                }).ToList(),

                Languages = entity.Languages?.Select(l => new LanguageItem
                {
                    LanguageName = l.LanguageName,
                    Level = l.Level
                }).ToList()
            }).ToList();
        }
    }
}
