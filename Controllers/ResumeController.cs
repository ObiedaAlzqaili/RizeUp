using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RizeUp.DTOs;
using RizeUp.Interfaces;
using RizeUp.Models;
using RizeUp.Repository;

namespace RizeUp.Controllers  
{
    public class ResumeController : Controller
    {
        private readonly IResumeOpenAiService _resumeOpenAiService;
        private readonly IResumeRepo _resumeRepo;

        public ResumeController(IResumeOpenAiService resumeOpenAiService, IResumeRepo resumeRepo)
        {
            _resumeOpenAiService = resumeOpenAiService;
           _resumeRepo = resumeRepo;
        }
  //      "firstName": "John",
  //"lastName": "Doe",
  //"phone": "+1-555-123-4567",
  //"email": "john.doe@example.com",
  //"linkedinLink": "https://www.linkedin.com/in/johndoe",
  //"gitHubLink": "https://github.com/johndoe",
  //"summary": "Experienced software developer with 5+ years of experience in full-stack development. Specialized in .NET Core, React, and cloud technologies. Strong problem-solving abilities and a track record of delivering high-quality solutions.",
  //"education": "Bachelor of Science in Computer Science\nMIT University\n2015-2019\nGPA: 3.8\n\nMaster of Science in Software Engineering\nStanford University\n2019-2021\nGPA: 3.9",
  //"experience": "Senior Software Engineer\nTech Corp International\nJan 2021 - Present\n- Led development of microservices architecture\n- Mentored junior developers\n- Implemented CI/CD pipelines\n\nSoftware Developer\nStartup Solutions Inc.\nJun 2019 - Dec 2020\n- Developed full-stack web applications\n- Optimized database performance\n- Implemented security best practices",
  //"skills": "Programming Languages: C#, JavaScript, Python\nFrameworks: .NET Core, React, Angular\nCloud: AWS, Azure\nDatabases: SQL Server, MongoDB\nTools: Git, Docker, Kubernetes",
  //"currentStep": 1
        public async Task<IActionResult> Index()
        {
            var resumes = await _resumeRepo.GetResumesByUserIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            return View(ToResumeJsonDtoList(resumes.ToList()));

        }

        public IActionResult NewResume()
        {
            return View(new CreateResumeDto { CurrentStep = 1 });
        }
        
        public IActionResult GeneratedResume(ResumeDto dto)
        {
            return View(dto);
        }
        
        [HttpGet]
        public async Task<IActionResult> EditResume(int resumeId)
        {
            var r = await _resumeRepo.GetResumeByIdAsync(resumeId);
            var resume = MapToResumeJsonDto(r);
            

            return View(resume);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> UpdateResume(ResumeDto resumeDto) // Rename parameter
        {
            if (!ModelState.IsValid)
            {
                return View("EditResume", resumeDto);
            }

            Resume resumeEntity = MapToResumeEntity(resumeDto, User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _resumeRepo.UpdateResumeAsync(resumeEntity); // Add await

            return RedirectToAction("Index"); // Redirect to reload data
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessStep(CreateResumeDto model, string command)
        {
            if (command == "prev")
            {
                model.CurrentStep--;
                return View("NewResume", model);
            }

            if (command == "next")
            {
                model.CurrentStep++;
                return View("NewResume", model);
            }
            else if (command == "submit")
            {
                // 1. Call the AI service, get back a sanitized ResumeJsonDto
                ResumeDto resumeDto = await _resumeOpenAiService.ParseResumeAsync(model);
                if (resumeDto != null)
                {
                    // 1. Get current user id (if using ASP.NET Identity)
                    string? userId = User?.Identity?.IsAuthenticated == true
                        ? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                        : null;

                    // 2. Map to entity
                    Resume resumeEntity = MapToResumeEntity(resumeDto, userId);
                    try
                    {
                        await _resumeRepo.AddResumeAsync(resumeEntity);
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions, e.g., log them
                        ModelState.AddModelError("", "An error occurred while saving the resume. Please try again.");
                        return View("NewResume", model);
                    }
                 
                }else
                {
                    ModelState.AddModelError("", "Failed to parse resume data. Please check your input and try again.");
                    return View("NewResume", model);
                }
            }

            // If something else happened, redisplay the form
            return View("NewResume", model);
        }
        private Resume MapToResumeEntity(ResumeDto dto, string userId)
        {
            return new Resume
            {
                ResumeId = dto.Id,
                Address = dto.Address, 
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Title = dto.Title,
                Summery = dto.Bio ?? dto.Bio, // use whichever is populated
                GitHubLink = dto.GitHubLink,
                LinkedinLink = dto.LinkedinLink,
                EndUserId = userId, // assuming you use ASP.NET Identity

                // Date/time fields:
                CreatedDate = DateTime.UtcNow.ToString(),
                ModifiedDate = DateTime.UtcNow.ToShortDateString(),

                // Collections, mapped (you may need to adjust for navigation properties):
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

        private ResumeDto MapToResumeJsonDto(Resume entity)
        {
            return new ResumeDto
            {
                Id = entity.ResumeId,
                GitHubLink = entity.GitHubLink,
                CreatedDate = entity.CreatedDate,
                ModifiedDate = entity.ModifiedDate,
                EndUserId = entity.EndUserId,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email,
                PhoneNumber = entity.PhoneNumber,
                Address = entity.Address,
                Bio = entity.Summery,
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

        private List<ResumeDto> ToResumeJsonDtoList(List<Resume> entities)
        {
            if (entities == null)
                return new List<ResumeDto>();

            return entities.Select(entity => new ResumeDto
            {
                Id = entity.ResumeId,
                GitHubLink = entity.GitHubLink,
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
