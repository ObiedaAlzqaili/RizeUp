// Services/CoverLetterAiService.cs
using System.Runtime.Intrinsics.X86;
using System.Text.Json;
using Microsoft.Identity.Client;
using Microsoft.SemanticKernel;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using RizeUp.DTOs;
using RizeUp.Interfaces;



public class AiLetterService : IAiLetterService
{
    private readonly Kernel _kernel;
    public AiLetterService(Kernel kernel) => _kernel = kernel;

    public async Task<CoverLetterResponseDto> GenerateAsync(CoverLetterRequestDto req)
    {
        // 1. Combine inputs into a single string with clear structure
        var rawText = $@"
[Job Description]
{req.JobDescription}

[Personal Information]
{req.PersonalInfo}
";

        // 2. Concise prompt with strict JSON output instructions
        var prompt = @"
Generate a cover letter as pure JSON using ONLY the following input:

{{$input}}

Output MUST be EXACTLY this JSON format with NO other text:
{
  ""ApplicantName"": ""string from Personal Information"",
  ""ApplicantAddress"": ""string from Personal Information"",
  ""ApplicantCityState"": ""string from Personal Information"",
  ""ApplicantEmail"": ""string from Personal Information"",
  ""ApplicantPhone"": ""string from Personal Information"",
  ""Date"": ""current date in MM/dd/yyyy format"",
  ""HiringManagerName"": ""string from Job Description"",
  ""CompanyName"": ""string from Job Description"",
  ""CompanyAddress"": ""string from Job Description"",
  ""CompanyCityState"": ""string from Job Description"",
  ""Paragraphs"": [""paragraph 1 text"", ""paragraph 2 text"", ""paragraph 3 text""],
  ""ClosingLine"": ""Sincerely,"",
  ""Signature"": ""[ApplicantName]""
}

    RULES:
1. Use today's date (MM/dd/yyyy)
2. Create exactly 3 paragraphs
3. Omit ANY property if information is missing
4. Output ONLY JSON with double quotes
";

    try
    {
        // 3. Call kernel with single input parameter
        var func = _kernel.CreateFunctionFromPrompt(prompt);
    var result = await func.InvokeAsync(_kernel, new KernelArguments
    {
        ["input"] = rawText
    });

    // 4. Extract and clean JSON response
    var jsonResponse = result.ToString();
    var cleanJson = ExtractPureJson(jsonResponse);

    // 5. Configure deserialization to match DTO defaults
    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true
    };

    // 6. Deserialize with null handling
    var dto = JsonSerializer.Deserialize<CoverLetterResponseDto>(cleanJson, options)
              ?? new CoverLetterResponseDto();

    // Ensure paragraphs list is never null
    dto.Paragraphs ??= new List<string>();
        
        return dto;
    }
    catch (Exception ex)
    {
        // Add logging here
        Console.WriteLine($"Generation error: {ex.Message}");
         return new CoverLetterResponseDto();
     }
}

private string ExtractPureJson(string rawResponse)
{
    // Handle common JSON wrapping patterns
    if (string.IsNullOrWhiteSpace(rawResponse)) return "{}";

    // Try to extract from markdown code block
    var jsonStart = rawResponse.IndexOf('{');
    var jsonEnd = rawResponse.LastIndexOf('}');

    if (jsonStart >= 0 && jsonEnd > jsonStart)
    {
        return rawResponse.Substring(jsonStart, jsonEnd - jsonStart + 1);
    }

    // Return empty JSON object if extraction fails
    return "{}";
}

    public async Task<ThankYouLetterResponseDto> GenerateThankYouAsync(string content)
    {
        // 1) Prompt template: instruct the AI to output ONLY the JSON matching our DTO
        var prompt = @"
    You are a professional thank-you letter writer. 
    Given this input context:
    ""{Context}""

    Return ONLY a single JSON object (no markdown, no commentary) that exactly matches this schema:
    {
      ""Subject"": string,
      ""InterviewerName"": string,
      ""BodyParagraphs"": string[],
      ""ClosingLine"": string,
      ""SenderName"": string,
      ""LinkedInProfile"": string,
      ""PhoneNumber"": string
    }";

        var func = _kernel.CreateFunctionFromPrompt(prompt);
        var result = await func.InvokeAsync(_kernel, new ()
        {
            ["input"] = content
        });

        // 3) Extract and clean JSON response
        var json = result.ToString().Trim();
        return JsonSerializer.Deserialize<ThankYouLetterResponseDto>(json)!;
    }
}
