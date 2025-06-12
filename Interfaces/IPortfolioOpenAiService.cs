using RizeUp.DTOs;

namespace RizeUp.Interfaces
{
    public interface IPortfolioOpenAiService
    {
        Task<PortfolioDto> ParsePortfolioDataAsync(CreatePortfolioDto dto);
    }
}
