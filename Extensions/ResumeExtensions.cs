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
                Bio = entity.Summery,
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
