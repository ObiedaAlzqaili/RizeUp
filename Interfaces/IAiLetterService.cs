using RizeUp.DTOs;

namespace RizeUp.Interfaces
{
    public interface IAiLetterService
    {
        Task<CoverLetterResponseDto> GenerateAsync(CoverLetterRequestDto req);
        Task<string> GenerateThankYouAsync(string content);

    }
}
