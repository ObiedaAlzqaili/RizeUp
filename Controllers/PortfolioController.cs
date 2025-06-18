using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MimeKit;
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
        private readonly EmailSettings _emailSettings;


        public PortfolioController(IPortfolioRepo portfolioRepo, IPortfolioOpenAiService portfolioOpenAiService , IOptions<EmailSettings> emailOptions )
        {
            
            _portfolioRepo = portfolioRepo;
            _portfolioOpenAiService = portfolioOpenAiService;
            _emailSettings = emailOptions.Value;
        }

  

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var portfolios = await _portfolioRepo.GetPortfoliosByUserIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            return View(MapToPortfolioJsonDtoList(portfolios.ToList()));
        }




        //Create New Portfolio
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
                if (command == "next" && model.CurrentStep == 1)
                {
                    if (model.PortfolioTemplateId == 0)
                    {
                        ModelState.AddModelError(nameof(model.PortfolioTemplateId),
                          "Please pick a template to continue.");
                        return View("NewPortfolio", model);
                    }
                }
                // Handle profile image upload and convert to base64 on step 1
                if (model.CurrentStep == 2 && model.ProfileImage != null && model.ProfileImage.Length > 0)
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
                else if (model.CurrentStep == 2 && model.ProfileImage == null && !string.IsNullOrEmpty(model.ProfileImageBase64))
                {
                    // Keep the existing image data
                }

                // Handle project images (if any) and convert to base64 on step 2
                if (model.CurrentStep == 3 && model.Projects != null)
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

                if (model.CurrentStep < 4)
                {
                    model.CurrentStep++;
                }
                return View("NewPortfolio", model);
            }

            // === SUBMIT button clicked on step 3 ===
            if (command == "submit" && model.CurrentStep == 4)
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
                await _portfolioRepo.AddPortfolioAsync(data);
                return RedirectToAction("Index");
            }

            // FALLBACK: stay on the same step
            return View("NewPortfolio", model);
        }


        //Edit on Resume
        [HttpGet]
        public async Task<IActionResult> EditPortfolio(int PortfolioId)
        {
            var portfolio = await _portfolioRepo.GetPortfolioByIdAsync(PortfolioId);
            var portfolioDto = MapToPortfolioDto(portfolio);
            return View(portfolioDto);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePortfolio(PortfolioDto portfolio)
        {
            if (!ModelState.IsValid)
            {
                // Re-render the view with validation errors and keep current step
                return View("EditPortfolio", portfolio);
            }
            var portfolio1 = MapToPortfolioEntity(portfolio, User.FindFirstValue(ClaimTypes.NameIdentifier));
            portfolio1.Id = portfolio.Id;
  
          await _portfolioRepo.UpdatePortfolioAsync(portfolio1);
            // After a successful edit, redirect to Index
            return RedirectToAction("Index");
        }



        //delete 
        [HttpGet]
        public async Task<IActionResult> DeletePortfolio(int portfolioId)
        {
            await _portfolioRepo.DeletePortfolioAsync(portfolioId);
           
            return RedirectToAction("Index");
        }


        //this for EMail Sender
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(int portfolioId, string name, string email, string subject, string message)
        {
            // 1. Look up the portfolio
            var portfolio = await _portfolioRepo.GetPortfolioByIdAsync(portfolioId);
            if (portfolio == null)
                return NotFound();

            // 2. Build the email with cleaner formatting
            var mime = new MimeMessage();
            mime.From.Add(new MailboxAddress(
                _emailSettings.FromName,
                _emailSettings.FromAddress));
            mime.To.Add(new MailboxAddress(
                $"{portfolio.FirstName} {portfolio.LastName}",
                portfolio.Email));
            mime.Subject = subject;

            // Clean, professional message formatting
            var body = new BodyBuilder
            {
                TextBody = $"Contact Form Submission\n" +
                          $"-----------------------\n" +
                          $"Name: {name}\n" +
                          $"Email: {email}\n\n" +
                          $"Message:\n" +
                          $"{message.Trim()}\n\n" +
                          $"-----------------------\n" +
                          $"Sent from your portfolio contact form"
            };
            mime.Body = body.ToMessageBody();

            // 3. Send via SMTP (keep your working code)
            using var client = new MailKit.Net.Smtp.SmtpClient();
            await client.ConnectAsync(
                _emailSettings.Host,
                _emailSettings.Port,
                SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(
                _emailSettings.Username,
                _emailSettings.Password);
            await client.SendAsync(mime);
            await client.DisconnectAsync(true);

            // 4. Confirm to user
            TempData["ContactSuccess"] = "Your message has been sent!";
            return RedirectToAction("Templates", new { ID = portfolioId });
        }


        //Temolate for Portdolio
        [HttpGet]
        public async Task<IActionResult> Templates(int portfolioId)
        {
            var portfolio = await _portfolioRepo.GetPortfolioByIdAsync(portfolioId);
            if (portfolio == null)
            {
                return NotFound();
            }
            var portfolioDto = MapToPortfolioDto(portfolio);
            return View(portfolioDto);
        }


        private Portfolio MapToPortfolioEntity(PortfolioDto dto, string userId)
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

        public List<PortfolioDto> MapToPortfolioJsonDtoList(List<Portfolio> portfolios)
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
        private PortfolioDto MapToPortfolioDto(Portfolio portfolio)
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

