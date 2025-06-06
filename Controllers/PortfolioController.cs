using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RizeUp.DTOs;
using RizeUp.Models;
using RizeUp.Repository;

namespace RizeUp.Controllers
{
    public class PortfolioController : Controller
    {
        //private readonly IPortfolioOpenAiService _portfolioOpenAiService;
        //private readonly IPortfolioRepo _portfolioRepo;

        //// Inject your AI service and your repository
        //public PortfolioController(IPortfolioRepo portfolioRepo)
        //{
        //    //_portfolioOpenAiService = portfolioOpenAiService;
        //    _portfolioRepo = portfolioRepo;
        //}
        private const string ViewName = "NewPortfolio";
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        //public IActionResult NewPortfolio()
        //{
        //    var dto = new CreatePortfolioDto
        //    {
        //        CurrentStep = 1
        //    };
        //    return View(dto);
        //}

        //// Handles all form submissions from the wizard (Next, Back, Submit)
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ProcessStep(CreatePortfolioDto model, string command)
        //{
        //    // If user clicked “Back”
        //    if (command == "prev")
        //    {
        //        if (model.CurrentStep > 1)
        //            model.CurrentStep--;
        //        return View("NewPortfolio", model);
        //    }

        //    // If user clicked “Next”
        //    if (command == "next")
        //    {
        //        // Perform server‐side validation for current step:
        //        //   e.g. if step 1, ensure those fields are valid before advancing.
        //        bool hasStep1Errors =
        //            ModelState[nameof(model.FirstName)]?.Errors.Count > 0 ||
        //            ModelState[nameof(model.LastName)]?.Errors.Count > 0 ||
        //            ModelState[nameof(model.Phone)]?.Errors.Count > 0 ||
        //            ModelState[nameof(model.Email)]?.Errors.Count > 0 ||
        //            ModelState[nameof(model.Summary)]?.Errors.Count > 0;

        //        if (hasStep1Errors)
        //        {
        //            // Re‐render the same step if validation fails
        //            return View("NewPortfolio", model);
        //        }

        //        if (model.CurrentStep < 3)
        //            model.CurrentStep++;
        //        return View("NewPortfolio", model);
        //    }

        //    // If user clicked “Enhance with AI” (Submit)
        //    if (command == "submit" && model.CurrentStep == 3)
        //    {
        //        // OPTIONAL: You could re‐validate fields from step 2 here:
        //        bool hasStep2Errors =
        //            ModelState[nameof(model.Services)]?.Errors.Count > 0 ||
        //            ModelState[nameof(model.Skills)]?.Errors.Count > 0;

        //        if (hasStep2Errors)
        //        {
        //            model.CurrentStep = 2;
        //            return View("NewPortfolio", model);
        //        }

        //        // 1. At this point, ModelState is valid for all pages. 
        //        // 2. You can now:
        //        //    - Save ProfileImage to disk or blob storage
        //        //    - Save each ProjectItemDto.Image
        //        //    - Persist the entire DTO to your database
        //        //    - Call your AI service to “enhance” the data, etc.

        //        // Redirect to a confirmation page or “My Portfolio” page:
        //        return RedirectToAction("Success");
        //    }

        //    // Fallback: re‐render current step
        //    return View("NewPortfolio", model);
        //}

        //// A simple Success page after submission
        //[HttpGet]
        //public IActionResult Success()
        //{
        //    return View();
        //}
        [HttpGet]
        public IActionResult NewPortfolio()
        {
            var dto = new CreatePortfolioDto
            {
                CurrentStep = 1
            };
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessStep(CreatePortfolioDto model, string command)
        {
            // === BACK button clicked ===
            if (command == "prev")
            {
                if (model.CurrentStep > 1)
                {
                    model.CurrentStep--;
                }
                return View(ViewName, model);
            }

            // === NEXT button clicked ===
            if (command == "next")
            {
                // Handle profile image upload and convert to base64 on step 1
                if (model.CurrentStep == 1 && model.ProfileImage != null && model.ProfileImage.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await model.ProfileImage.CopyToAsync(ms);
                        var bytes = ms.ToArray();
                        model.ProfileImageBase64 = Convert.ToBase64String(bytes);
                    }
                    model.ProfileImage = null;
                }

                // Handle project images (if any) and convert to base64 on step 2
                if (model.CurrentStep == 2 && model.Projects != null)
                {
                    for (int i = 0; i < model.Projects.Count; i++)
                    {
                        var project = model.Projects[i];
                        if (project.Image != null && project.Image.Length > 0)
                        {
                            using (var ms = new MemoryStream())
                            {
                                await project.Image.CopyToAsync(ms);
                                var bytes = ms.ToArray();
                                project.ImageBase64 = Convert.ToBase64String(bytes);
                            }
                            project.Image = null;
                        }
                    }
                }

                // Validate only the fields for the current step
                if (model.CurrentStep == 1)
                {
                    var step1Fields = new[]
                    {
                        nameof(model.FirstName),
                        nameof(model.LastName),
                        nameof(model.Phone),
                        nameof(model.Email),
                        nameof(model.Summary)
                    };

                    foreach (var field in step1Fields)
                    {
                        if (ModelState[field]?.Errors?.Count > 0)
                        {
                            return View(ViewName, model);
                        }
                    }
                }
                else if (model.CurrentStep == 2)
                {
                    var step2Fields = new[]
                    {
                        nameof(model.Services),
                        nameof(model.Skills)
                    };

                    foreach (var field in step2Fields)
                    {
                        if (ModelState[field]?.Errors?.Count > 0)
                        {
                            model.CurrentStep = 2;
                            return View(ViewName, model);
                        }
                    }

                    if (model.Projects != null)
                    {
                        for (int i = 0; i < model.Projects.Count; i++)
                        {
                            var prefix = $"Projects[{i}]";
                            if (ModelState[$"{prefix}.{nameof(ProjectItemDto.Name)}"]?.Errors?.Count > 0 ||
                                ModelState[$"{prefix}.{nameof(ProjectItemDto.Description)}"]?.Errors?.Count > 0)
                            {
                                model.CurrentStep = 2;
                                return View(ViewName, model);
                            }
                        }
                    }
                }

                if (model.CurrentStep < 3)
                {
                    model.CurrentStep++;
                }
                return View(ViewName, model);
            }

            // === SUBMIT button clicked on step 3 ===
            if (command == "submit" && model.CurrentStep == 3)
            {
                // At this point, ProfileImageBase64 and each Project.ImageBase64 contain the image data if uploaded

                // TODO: Save ProfileImageBase64 and Project.ImageBase64 to disk/blob or process as needed
                // TODO: Map 'model' → your database entity and persist
                // TODO: (Optional) Call your AI service to “enhance” the portfolio

                return RedirectToAction(nameof(Success));
            }

            // FALLBACK: stay on the same step
            return View(ViewName, model);
        }

        [HttpGet]
        public IActionResult Success()
        {
            return View();
        }

        /// <summary>
        /// GET /Portfolio/GeneratedPortfolio  
        /// Shows the final, AI‐enhanced portfolio (exactly like Resume did with GeneratedResume).
        /// </summary>
        //public IActionResult GeneratedPortfolio(PortfolioJsonDto dto)
        //{
        //    return View(dto);
        //}
    }
    
}

