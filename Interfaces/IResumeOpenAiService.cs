using RizeUp.DTOs;
using RizeUp.Models;

namespace RizeUp.Interfaces
{
    public interface IResumeOpenAiService
    {
        Task<ResumeDto> ParseResumeAsync(CreateResumeDto resemeRowData);
    }
}
