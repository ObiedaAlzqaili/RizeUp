using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using RizeUp.DTOs;
using RizeUp.Interfaces;


namespace RizeUp.Services
{
    public class ResumeOpenAiService : IResumeOpenAiService
    {
        private readonly Kernel _kernel;

        public ResumeOpenAiService(Kernel kernel)
        {
            _kernel = kernel ;
        }

        public async Task<ResumeDto> ParseResumeAsync(CreateResumeDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            // 1. Build the same prompt you already have:
            //string prompt = @"
            //You are a helpful AI assistant whose job is to take raw resume/ form data(provided below)
            //and output a single, valid JSON object in English only. If any part of the input is in another
            //language, translate it into English before proceeding.Use all details you can glean from the
            //Summary field to craft a strong, concise personal headline and a powerful professional summary.
            //Enhance or expand any sparse content to make it more impactful, but do not invent credentials the
            //user never provided.

            //Below is the EXACT schema for the JSON you must produce.Do not add any extra keys.Fill in
            //every key; if a section has no data, use null(for single‐value fields) or an empty array(for lists).

            //{
            //    ""Title"": ""string"",
            //""FirstName"": ""string"",
            //""LastName"": ""string"",
            //""Email"": ""string"",
            //""PhoneNumber"": ""string"",
            //""LinkedinLink"": ""string or null"",
            //""GitHubLink"": ""string or null"",
            //""Summary"": ""string"",
            //""Educations"": [
            //{
            //        ""CollegeName"": ""string"",
            //""DegreeField"": ""string"",
            //""Major"": ""string"",
            //""StartDate"": ""string"",
            //""EndDate"": ""string"",
            //""GPA"": number(or null)
            //}
            //],
            //""Experiences"": [
            //{
            //        ""Title"": ""string"",
            //""Company"": ""string"",
            //""StartDate"": ""string"",
            //""EndDate"": ""string"",
            //""IsCurrent"": boolean(or null),
            //""Duties"": ""string""
            //}
            //],
            //""Skills"": [
            //{ ""SkillName"": ""string"", ""SkillType"": ""string"" }, ...
            //],
            //""Languages"": [
            //{
            //        ""LanguageName"": ""string"",
            //""Level"": ""string""
            //}
            //],
            //""Certificates"": [
            //{
            //        ""ProviderName"": ""string"",
            //""Field"": ""string"",
            //""StartDate"": ""string"",
            //""EndDate"": ""string"",
            //""GPA"": number(or null)
            //}
            //],
            //""Projects"": [
            //{
            //        ""ProjectName"": ""string"",
            //""ProjectDescription"": ""string"",
            //""StartDate"": ""string"",
            //""EndDate"": ""string"",
            //""ProjectLink"": ""string""
            //}
            //]
            //}

                //Below is the raw user input. Insert it exactly where {{input}} appears.
                //{{input}}

                //IMPORTANT: Output only the JSON—no extra text. Begin output now (just the JSON):
                //";

            var prompt = @"
            You are a helpful AI assistant whose job is to take raw resume/ form data(provided below)
            and output a single, valid JSON object in English only. If any part of the input is in another
            language, translate it into English before proceeding.Use all details you can glean from the
            Summary field to craft a strong, concise personal headline and a powerful professional summary.
            Enhance or expand any sparse content to make it more impactful, but do not invent credentials the
            user never provided.

            Below is the EXACT schema for the JSON you must produce.Do not add any extra keys.Fill in
            every key; if a section has no data, use null(for single‐value fields) or an empty array(for lists).

Convert the following unstructured CV text into JSON with the structure:
{
            ""Title"": ""string"",
            ""FirstName"": ""string"",
            ""LastName"": ""string"",
            ""Email"": ""string"",
            ""PhoneNumber"": ""string"",
            ""LinkedinLink"": ""string or null"",
            ""GitHubLink"": ""string or null"",
            ""Summary"": ""string"",
            ""Educations"": [
            {
            ""CollegeName"": ""string"",
            ""IsCurrent"": ""boolean(or null)"",
            ""DegreeType"": ""string"",
            ""Major"": ""string"",
            ""StartDate"": ""string"",
            ""EndDate"": ""string"",
            ""GPA"": number(or null)
            }
            ],
            ""Experiences"": [
            {
            ""Title"": ""string"",
            ""Company"": ""string"",
            ""StartDate"": ""string"",
            ""EndDate"": ""string"",
            ""IsCurrent"": boolean(or null),
            ""Duties"": ""string""
            }
            ],
            ""Skills"": [
            { ""SkillName"": ""string"", ""SkillType"": ""string"" }, ...
            ],
            ""Languages"": [
            {
                    ""LanguageName"": ""string"",
            ""Level"": ""string""
            }
            ],
            ""Certificates"": [
            {
            ""Title"": ""string"",
            ""ProviderName"": ""string"",
            ""Field"": ""string"",
            ""StartDate"": ""string"",
            ""EndDate"": ""string"",
            ""GPA"": number(or null)
            }
            ],
            ""Projects"": [
            {
            ""ProjectName"": ""string"",
            ""IsOngoing"": ""boolean(or null)"",
            ""ProjectDescription"": ""string"",
            ""StartDate"": ""string"",
            ""EndDate"": ""string"",
            ""ProjectLink"": ""string""
            }
            ]
            }
IF any field is not present, use Not null as the value and if it was array use empty array [].
CV TEXT:
{{$input}}

JSON:
";

            // 2. Create a “single‐string” prompt function:


            //3.Build rawText by concatenating each DTO property, one per line:
                        string rawText = $@"
                          FirstName: {dto.FirstName ?? ""}
                          LastName: {dto.LastName ?? ""}
                          Email: {dto.Email ?? ""}
                          Phone: {dto.Phone ?? ""}
                          LinkedinLink: {dto.LinkedinLink ?? ""}
                          GitHubLink: {dto.GitHubLink ?? ""}
                          Summary: {(dto.Summary ?? "").Replace("\r\n", " ").Replace("\n", " ")}
                          Education: {(dto.Education ?? "").Replace("\r\n", "; ").Replace("\n", "; ")}
                          Experience: {(dto.Experience ?? "").Replace("\r\n", "; ").Replace("\n", "; ")}
                          Skills: {(dto.Skills ?? "").Replace("\r\n", ", ").Replace("\n", ", ")}
            ";

      
            //John
            //Doe
            //john.doe@example.com
            //+1 (555) 123-4567
            //https://www.linkedin.com/in/johndoe
            //https://github.com/johndoe
            //Seasoned software engineer with over 8 years of experience building scalable web applications and microservices.
            //Expert in .NET Core, cloud architectures, and agile methodologies.
            //Passionate about leading teams and driving best practices in software design.
            //Massachusetts Institute of Technology, Bachelor of Science, Computer Science, 2010-09, 2014-06, 3.9; Stanford University, Master of Science, Computer Science (Artificial Intelligence), 2015-09, 2017-06, 4.0
            //Software Engineer, Acme Corp, 2017-07, 2020-08, false, Developed and maintained ASP.NET Core microservices for high-traffic e-commerce platform; Senior Software Engineer, Beta Inc, 2020-09, Present, true, Leading a team of 5 engineers to architect cloud-native solutions on Azure, introducing CI/CD pipelines, and improving system reliability
            //C#, ASP.NET Core, Entity Framework, Azure, Docker, Kubernetes, JavaScript, React, SQL, Git
            //";

            KernelFunction extractFunction = _kernel.CreateFunctionFromPrompt(prompt);

            FunctionResult result = await _kernel.InvokeAsync(extractFunction, new()
            {
                ["input"] = rawText
            });

          
            string json = result.ToString();

         
            ResumeDto resumeDto = JsonSerializer.Deserialize<ResumeDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new ResumeDto();

            
            resumeDto.FirstName = resumeDto.FirstName ?? "";
            resumeDto.Title = resumeDto.Title ?? "";
            resumeDto.LastName = resumeDto.LastName ?? "";
            resumeDto.Email = resumeDto.Email ?? "";
            resumeDto.PhoneNumber = resumeDto.PhoneNumber ?? "";
            resumeDto.Address = resumeDto.Address ?? "";
            resumeDto.Summary = resumeDto.Summary ?? "";
            resumeDto.Title = resumeDto.Title ?? "";
            resumeDto.GitHubLink = resumeDto.GitHubLink ?? "";
            resumeDto.LinkedinLink = resumeDto.LinkedinLink ?? "";
            resumeDto.Educations ??= new List<EducationItem>();
            resumeDto.Experiences ??= new List<ExperienceItem>();
            resumeDto.Skills ??= new List<SkillItem1>();
            resumeDto.Languages ??= new List<LanguageItem>();
            resumeDto.Certificates ??= new List<CertificateItem>();

         
            return resumeDto;
        }
    }
}
