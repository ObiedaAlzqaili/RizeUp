using Microsoft.AspNetCore.Mvc;
using RizeUp.DTOs;
using RizeUp.Interfaces;   
using RizeUp.Repository;

namespace RizeUp.Controllers
{
    public class ResumeController : Controller
    {
        private readonly IResumeOpenAiService _resumeOpenAiService;

        public ResumeController(IResumeOpenAiService resumeOpenAiService)
        {
            _resumeOpenAiService = resumeOpenAiService;
        }

        public IActionResult Index()
        {
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

                //// 2. Pass it straight into the "GeneratedResume" view
                //return View("GeneratedResume", resumeDto);
                TempData["ResumeJson"] = System.Text.Json.JsonSerializer.Serialize(resumeDto);
                return Json(new { redirectUrl = Url.Action("RenderResume") });
            }

            // If something else happened, redisplay the form
            return View("NewResume", model);
        }
        public IActionResult RenderResume()
        {
            if (!TempData.ContainsKey("ResumeJson")) return RedirectToAction("NewResume");

            var resumeJson = TempData["ResumeJson"]?.ToString();
            if (string.IsNullOrEmpty(resumeJson)) return RedirectToAction("NewResume");

            var model = System.Text.Json.JsonSerializer.Deserialize<ResumeJsonDto>(resumeJson);
            return View("GeneratedResume", model);
        }
    }
}
