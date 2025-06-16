using Microsoft.EntityFrameworkCore;
using RizeUp.Data;
using RizeUp.Models;

namespace RizeUp.Repository
{
    public class PortfolioRepo : IPortfolioRepo
    {
        private readonly ApplicationDbContext _context;

        public PortfolioRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Portfolios.CountAsync();
        }

        //public async Task<List<PortfolioReview>> GetRecentReviewsAsync(int count)
        //{
        //    return await _context.Portfolio
        //        .Include(r => r.User)
        //        .OrderByDescending(r => r.ReviewDate)
        //        .Take(count)
        //        .Select(r => new PortfolioReview
        //        {
        //            UserName = r.User.UserName,
        //            Title = r.User.Title ?? "User",
        //            Content = r.Content,
        //            ReviewDate = r.ReviewDate
        //        })
        //        .ToListAsync();
        //}

        public async Task AddPortfolioAsync(Portfolio portfolio)
        {
            if (portfolio == null)
                throw new ArgumentNullException(nameof(portfolio), "Portfolio cannot be null");

            _context.Portfolios.Add(portfolio);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePortfolioAsync(int id)
        {
            var portfolio = await _context.Portfolios
                .Include(p => p.Projects)
                .Include(p => p.Skills)
                .Include(p => p.Services)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (portfolio != null)
            {
                portfolio.IsDeleted = true; // Soft delete
                portfolio.ModifiedDate = DateOnly.FromDateTime(DateTime.Now).ToString();

                // Optionally, soft delete children too, if you have IsDeleted on child entities
                // portfolio.Projects.ForEach(p => p.IsDeleted = true);
                // etc.

                _context.Portfolios.Update(portfolio);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Portfolio with ID {id} not found.");
            }
        }

        public async Task<Portfolio> GetPortfolioByIdAsync(int id)
        {
            var por = await _context.Portfolios
                .AsNoTracking()
                .Include(p => p.Skills)
                .Include(p => p.Projects)
                .Include(p => p.Services)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (por == null)
                throw new KeyNotFoundException($"Portfolio with ID {id} not found.");
            if (por.IsDeleted)
                throw new InvalidOperationException($"Portfolio with ID {id} is deleted.");

            return por;
        }

        public async Task<IEnumerable<Portfolio>> GetPortfoliosByUserIdAsync(string userId)
        {
            return await _context.Portfolios
                .Where(p => p.EndUserId == userId && !p.IsDeleted)
                .ToListAsync();
        }

        public async Task UpdatePortfolioAsync(Portfolio portfolio)
        {
            if (portfolio == null)
                throw new ArgumentNullException(nameof(portfolio), "Portfolio cannot be null");

            var existingPortfolio = await _context.Portfolios
                .Include(p => p.Services)
                .Include(p => p.Projects)
                .Include(p => p.Skills)
                .FirstOrDefaultAsync(p => p.Id == portfolio.Id);

            if (existingPortfolio == null)
                throw new KeyNotFoundException($"Portfolio with ID {portfolio.Id} not found.");

            // Update main properties
            _context.Entry(existingPortfolio).CurrentValues.SetValues(portfolio);
            existingPortfolio.ModifiedDate = DateOnly.FromDateTime(DateTime.Now).ToString();

            // Remove all existing children
            _context.Services.RemoveRange(existingPortfolio.Services ?? new List<Service>());
            _context.Projects.RemoveRange(existingPortfolio.Projects ?? new List<Project>());
            _context.Skills.RemoveRange(existingPortfolio.Skills ?? new List<Skill>());

            // Add the new children (if any)
            existingPortfolio.Services = portfolio.Services ?? new List<Service>();
            existingPortfolio.Projects = portfolio.Projects ?? new List<Project>();
            existingPortfolio.Skills = portfolio.Skills ?? new List<Skill>();

            await _context.SaveChangesAsync();
        }
    }
}
