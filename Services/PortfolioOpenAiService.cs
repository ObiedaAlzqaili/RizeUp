using System.Text.Json;
using Microsoft.SemanticKernel;
using RizeUp.DTOs;
using RizeUp.Interfaces;

namespace RizeUp.Services
{
    public class PortfolioOpenAiService : IPortfolioOpenAiService
    {
        private readonly Kernel _kernel;

        public PortfolioOpenAiService(Kernel kernel)
        {
            _kernel = kernel;
        }

        public async Task<PortfolioJsonDto> ParsePortfolioDataAsync(CreatePortfolioDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var prompt = @"
    You are a helpful AI assistant whose job is to take raw portfolio form data (provided below)
    and output a single, valid JSON object in English only. If any part of the input is in another
    language, translate it into English before proceeding. Use all details you can glean from the
    Summary field to craft a strong, concise personal headline and a powerful professional summary.
    Enhance or expand any sparse content to make it more impactful, but do not invent credentials the
    user never provided.

    You are a JSON parser for portfolio data.  
    Given the following raw key:value lines, output a single JSON object matching this schema exactly (omit any other keys):

    {
      ""title"": string or null,
      ""firstName"": string,
      ""lastName"": string,
      ""email"": string,
      ""phoneNumber"": string,
      ""address"": string or null,
      ""summery"": string,
      ""gitHubLink"": string or null,
      ""linkedinLink"": string or null,
      ""services"": [
        { ""serviceName"": string or null, ""serviceDescription"": string or null }
      ] or [],
      ""projects"": [
        {
          ""projectName"": string or null,
          ""projectDescription"": string or null,
          ""startDate"": string or null,
          ""endDate"": string or null,
          ""isOngoing"": boolean or null,
          ""projectLink"": string or null
        }
      ] or []
    }

    If any section is empty or cannot be parsed, emit `[]`.  
    Return _only_ the JSON—no commentary.

    RAW DATA:
    {{$input}}

    JSON:
    ";

            // Build rawText from the CreatePortfolioDto
            var rawText = $@"
   
    FirstName: {dto.FirstName}
    LastName: {dto.LastName}
    Email: {dto.Email}
    PhoneNumber: {dto.Phone}
    Address: 
    Summery: {dto.Summary?.Replace("\r\n", " ").Replace("\n", " ")}
    GitHubLink: {dto.GitHub ?? ""}
    LinkedinLink: {dto.LinkedIn ?? ""}
    Services: {dto.Services?.Replace("\r\n", "; ").Replace("\n", "; ")}
    Projects: {string.Join(" | ", dto.Projects?.Select(p =>
    $"Name:{p.Name};Desc:{p.Description};Link:{p.Link}") ?? Array.Empty<string>())}
    ";

            KernelFunction extractFunction = _kernel.CreateFunctionFromPrompt(prompt);

            FunctionResult result = await _kernel.InvokeAsync(extractFunction, new()
            {
                ["input"] = rawText
            });

            string json = result.ToString();

            PortfolioJsonDto portfolioDto;
            try
            {
                portfolioDto = JsonSerializer.Deserialize<PortfolioJsonDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new PortfolioJsonDto();
            }
            catch
            {
                portfolioDto = new PortfolioJsonDto();
            }

            // Fill in any missing fields from the original dto if needed
            portfolioDto.FirstName ??= dto.FirstName;
            portfolioDto.LastName ??= dto.LastName;
            portfolioDto.Email ??= dto.Email;
            portfolioDto.PhoneNumber ??= dto.Phone;
            portfolioDto.LinkedinLink ??= dto.LinkedIn;
            portfolioDto.GitHubLink ??= dto.GitHub;
            portfolioDto.Summery ??= dto.Summary;
            portfolioDto.ImageBase64 ??= dto.ProfileImageBase64;
            portfolioDto.ImageFileName ??= dto.ProfileImageFileName;
            portfolioDto.ImageContentType ??= dto.ProfileImageContentType;

            return portfolioDto;
        }
    }
}
