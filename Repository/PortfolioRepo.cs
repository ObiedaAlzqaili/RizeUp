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
                    //_context.Portfolios.Remove(portfolio);
                    
                    portfolio.IsDeleted = true; // Soft delete
                    portfolio.ModifiedDate = DateOnly.FromDateTime(DateTime.Now);
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
            Portfolio por;
            try
            {
                por = await _context.Portfolios.FindAsync(id);
                if (por == null)
                {
                    throw new KeyNotFoundException($"Portfolio with ID {id} not found.");
                }
                else if ((bool)por.IsDeleted)
                {
                    throw new InvalidOperationException($"Portfolio with ID {id} is deleted.");
                }

                }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the portfolio.", ex);
            }
            return por;

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
                var existingPortfolio = await _context.Portfolios.FindAsync(portfolio.Id);
                if (existingPortfolio == null)
                {
                    throw new KeyNotFoundException($"Portfolio with ID {portfolio.Id} not found.");
                }
                existingPortfolio.PersonalImage = portfolio.PersonalImage;
                existingPortfolio.Services = portfolio.Services;
                existingPortfolio.Projects = portfolio.Projects;
                existingPortfolio.IsDeleted = portfolio.IsDeleted;
                existingPortfolio.EndUserId = portfolio.EndUserId;
                existingPortfolio.ModifiedDate = DateOnly.FromDateTime(DateTime.Now);
                _context.Portfolios.Update(existingPortfolio);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the portfolio.", ex);
            }

        }
    }
}
