using RizeUp.DTOs;

namespace RizeUp.Interfaces
{
    public interface IPortfolioOpenAiService
    {
        Task<PortfolioJsonDto> ParsePortfolioDataAsync(CreatePortfolioDto dto);
    }
}
