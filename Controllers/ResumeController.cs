using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using RizeUp.DTOs;
using RizeUp.Interfaces;
using RizeUp.Models;
using RizeUp.Repository;
using RizeUp.Services.PdfGeneration;

namespace RizeUp.Controllers  
{
    public class ResumeController : Controller
    {
        private readonly IResumeOpenAiService _resumeOpenAiService;
        private readonly IResumeRepo _resumeRepo;
        private readonly IResumePdfGenerator _pdfGenerator;
        public ResumeController(IResumeOpenAiService resumeOpenAiService, IResumeRepo resumeRepo,IResumePdfGenerator pdfGenerator)
        {
            _resumeOpenAiService = resumeOpenAiService;
           _resumeRepo = resumeRepo;
            _pdfGenerator = pdfGenerator;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var resumes = await _resumeRepo.GetResumesByUserIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            return View(ToResumeJsonDtoList(resumes.ToList()));

        }
        //edit
        public IActionResult NewResume()
        {
            return View(new CreateResumeDto { CurrentStep = 1 });
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
              
                ResumeDto resumeDto = await _resumeOpenAiService.ParseResumeAsync(model);
                if (resumeDto != null)
                {
                    
                    string? userId = User?.Identity?.IsAuthenticated == true
                        ? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                        : null;

                   
                    Resume resumeEntity = MapToResumeEntity(resumeDto, userId);
                    try
                    {
                        await _resumeRepo.AddResumeAsync(resumeEntity);
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        
                        ModelState.AddModelError("", "An error occurred while saving the resume. Please try again.");
                        return View("NewResume", model);
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Failed to parse resume data. Please check your input and try again.");
                    return View("NewResume", model);
                }
            }

            // If something else happened, redisplay the form
            return View("NewResume", model);
        }


        
        // edit
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


        //Delete
        public async Task<IActionResult> DeleteResume(int resumeId )
        {
            await _resumeRepo.DeleteResumeAsync(resumeId);
            return RedirectToAction("Index");
        }


        [HttpGet("DownloadResume")]
        public async Task<IActionResult> DownloadResume(int resumeId)
        {
            var resumeEntity = await _resumeRepo.GetResumeByIdAsync(resumeId);
            if (resumeEntity == null)
            {
                return NotFound();
            }

            var resumeDto = MapToResumeJsonDto(resumeEntity);

            try
            {
                var pdfBytes = _pdfGenerator.GenerateResumePdf(resumeDto);
                return File(pdfBytes, "application/pdf",
                    $"{resumeDto.FirstName}_{resumeDto.LastName}_Resume.pdf");
            }
            catch (Exception ex)
            {
                // Log error here
                return StatusCode(500, $"PDF generation failed: {ex.Message}");
            }
        }
        

        //template for resume
        public async Task<IActionResult> Templates(int resumeId)
        {
            var r = await _resumeRepo.GetResumeByIdAsync(resumeId);
            var dto = MapToResumeJsonDto(r);
            return View(dto);
        }














        private Resume MapToResumeEntity(ResumeDto dto, string userId)
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
                Summery = dto.Summary ?? dto.Summary, // use whichever is populated
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

        private List<ResumeDto> ToResumeJsonDtoList(List<Resume> entities)
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
