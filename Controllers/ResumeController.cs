using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RizeUp.DTOs;
using RizeUp.Interfaces;
using RizeUp.Models;
using RizeUp.Repository;

namespace RizeUp.Controllersء   
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

        public async Task<IActionResult> Index()
        {
            // This could be a list of resumes for the current user, or a welcome page
            // If you want to show all resumes, you might need to fetch them from the database
            // For now, just return a view
            // You might want to pass a list of resumes to the view
            //var resumes = await _resumeRepo.GetResumesByUserIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return View();

        }

        public IActionResult NewResume()
        {
            return View(new CreateResumeDto { CurrentStep = 1 });
        }
        
        public IActionResult GeneratedResume(ResumeJsonDto dto)
        {
            return View(dto);
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
                ResumeJsonDto resumeDto = await _resumeOpenAiService.ParseResumeAsync(model);
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
        private Resume MapToResumeEntity(ResumeJsonDto dto, string userId)
        {
            return new Resume
            {
                // Top-level properties:
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

        
    }
}
