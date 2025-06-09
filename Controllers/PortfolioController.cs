using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using RizeUp.DTOs;
using RizeUp.Interfaces;
using RizeUp.Models;
using RizeUp.Repository;

namespace RizeUp.Controllers
{
    public class PortfolioController : Controller
    {
        private readonly IPortfolioOpenAiService _portfolioOpenAiService;
        private readonly IPortfolioRepo _portfolioRepo;

  
        public PortfolioController(IPortfolioRepo portfolioRepo, IPortfolioOpenAiService portfolioOpenAiService)
        {
            
            _portfolioRepo = portfolioRepo;
            _portfolioOpenAiService = portfolioOpenAiService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var portfolios = await _portfolioRepo.GetPortfoliosByUserIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            return View(MapToPortfolioJsonDtoList(portfolios.ToList()));
        }

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
                return View("NewPortfolio", model);
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
                        model.ProfileImageFileName = model.ProfileImage.FileName;
                        model.ProfileImageContentType = model.ProfileImage.ContentType;
                    }
                    model.ProfileImage = null; // Clear the file to avoid model binding issues
                }
                // If no new image was uploaded but we have existing base64 data, retain it
                else if (model.CurrentStep == 1 && model.ProfileImage == null && !string.IsNullOrEmpty(model.ProfileImageBase64))
                {
                    // Keep the existing image data
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
                                project.ImageFileName = project.Image.FileName;
                                project.ImageContentType = project.Image.ContentType;
                            }
                            project.Image = null; // Clear the file to avoid model binding issues
                        }
                        // If no new image was uploaded but we have existing base64 data, retain it
                        else if (project.Image == null && !string.IsNullOrEmpty(project.ImageBase64))
                        {
                            // Keep the existing image data
                        }
                    }
                }

                // Rest of the validation logic remains the same...
                // [Previous validation code here]

                if (model.CurrentStep < 3)
                {
                    model.CurrentStep++;
                }
                return View("NewPortfolio", model);
            }

            // === SUBMIT button clicked on step 3 ===
            if (command == "submit" && model.CurrentStep == 3)
            {
                var result = await _portfolioOpenAiService.ParsePortfolioDataAsync(model);
                if (result == null)
                {
                    ModelState.AddModelError("", "Failed to generate portfolio. Please try again.");
                    return View("NewPortfolio", model);
                }

                // Save the portfolio data to the database
                // You may need to map the DTO to your Portfolio entity/model
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var data = MapToPortfolioEntity(result, userId);
                await _portfolioRepo.AddPortfolioAsync(data); ;
                return RedirectToAction("Index");
            }

            // FALLBACK: stay on the same step
            return View("NewPortfolio", model);
        }

        [HttpGet]
        public IActionResult PortfolioTemplate1(PortfolioJsonDto json)
        {
            
            return View(json);
        }

        private Portfolio MapToPortfolioEntity(PortfolioJsonDto dto, string userId)
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
                    EndDate = string.IsNullOrWhiteSpace(p.EndDate) ? null :p.EndDate,
                    IsOngoing = p.IsOngoing,
                    ProjectLink = p.ProjectLink
                }).ToList() ?? new List<Project>()
            };
        }

        public List<PortfolioJsonDto> MapToPortfolioJsonDtoList(List<Portfolio> portfolios)
        {
            if (portfolios == null || portfolios.Count == 0)
                return new List<PortfolioJsonDto>();

            return portfolios.Select(p => new PortfolioJsonDto
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

