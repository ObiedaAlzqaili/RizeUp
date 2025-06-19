using RizeUp.Models;

namespace RizeUp.Repository
{
    public interface IPortfolioRepo
    {
        Task<Portfolio> GetPortfolioByIdAsync(int id);
        
        Task AddPortfolioAsync(Portfolio portfolio);
        Task UpdatePortfolioAsync(Portfolio portfolio);
        Task DeletePortfolioAsync(int id);
        Task<IEnumerable<Portfolio>> GetPortfoliosByUserIdAsync(string userId);
        Task<int> GetCountAsync();

        Task<List<Portfolio>> GetPortfoliosCount(int count);
        //Task<List<PortfolioReview>> GetRecentReviewsAsync(int count);

    }
}
