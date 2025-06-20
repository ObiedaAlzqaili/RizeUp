using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using RizeUp.DTOs;
using RizeUp.Interfaces;
using RizeUp.Models;
using RizeUp.Repository;
using RizeUp.Services.PdfGeneration;
using RizeUp.Extensions;

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
            
            return View(ResumeExtensions.ToResumeJsonDtoList(resumes.ToList()));
            //ToResumeJsonDtoList(resumes.ToList())

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


                    Resume resumeEntity = ResumeExtensions.MapToResumeEntity(resumeDto, userId);
                        //MapToResumeEntity(resumeDto, userId);
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
            var resume = ResumeExtensions.MapToResumeJsonDto(r);
                         
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
            Resume resumeEntity = ResumeExtensions.MapToResumeEntity(resumeDto, User.FindFirstValue(ClaimTypes.NameIdentifier)); 
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

            var resumeDto = ResumeExtensions.MapToResumeJsonDto(resumeEntity);

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
            var dto = ResumeExtensions.MapToResumeJsonDto(r);
            return View(dto);
        }



    }
}
