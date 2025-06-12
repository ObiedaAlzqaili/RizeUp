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

        public async Task AddPortfolioAsync(Portfolio portfolio)
        {
            try
            {
                _context.Portfolios.Add(portfolio);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the portfolio.", ex);
            }
        }

        public async Task DeletePortfolioAsync(int id)
        {
            try
            {
                var portfolio = await _context.Portfolios.FindAsync(id);
                if (portfolio != null)
                {
                    portfolio.IsDeleted = true; // Soft delete
                    portfolio.ModifiedDate = DateOnly.FromDateTime(DateTime.Now).ToString();
                    _context.Portfolios.Update(portfolio);
                    _context.Projects.RemoveRange(_context.Projects.Where(p => p.PortfolioId == id));
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the portfolio.", ex);
            }
        }

        public async Task<Portfolio> GetPortfolioByIdAsync(int id)
        {
            try
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
            catch (DbUpdateException ex)
            {
                throw new Exception("A database error occurred while retrieving the portfolio.", ex);
            }
        }

        public async Task<IEnumerable<Portfolio>> GetPortfoliosByUserIdAsync(string userId)
        {
            List<Portfolio> portfolios;
            try
            {
                portfolios = await _context.Portfolios
                    .Where(p => p.EndUserId == userId && !p.IsDeleted)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving portfolios by user ID.", ex);
            }
            return portfolios;
        }

        public async Task UpdatePortfolioAsync(Portfolio portfolio)
        {
            try
            {
                var existingPortfolio = await _context.Portfolios
                    .Include(p => p.Services)
                    .Include(p => p.Projects)
                    .Include(p => p.Skills)
                    .FirstOrDefaultAsync(p => p.Id == portfolio.Id);

                if (existingPortfolio == null)
                {
                    throw new KeyNotFoundException($"Portfolio with ID {portfolio.Id} not found.");
                }

                // Remove related entities
                _context.Services.RemoveRange(existingPortfolio.Services);
                _context.Projects.RemoveRange(existingPortfolio.Projects);
                _context.Skills.RemoveRange(existingPortfolio.Skills);

                // Remove the portfolio itself
                _context.Portfolios.Remove(existingPortfolio);

                // Add the new portfolio
                _context.Portfolios.Add(portfolio);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the portfolio.", ex);
            }
        }
    }
}
