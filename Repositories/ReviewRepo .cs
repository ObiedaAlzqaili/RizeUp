using Microsoft.EntityFrameworkCore;
using RizeUp.Data;
using RizeUp.Models;

namespace RizeUp.Repositories
{
    public class ReviewRepo : IReviewRepo
    {
        private readonly ApplicationDbContext _context;
        public ReviewRepo(ApplicationDbContext ctx) => _context = ctx;

        public async Task AddAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var review = new Review { Id = id };
            _context.Reviews.Attach(review);
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }

        public Task<int> GetCountAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Review>> GetRecentAsync(int count = 5) =>
            await _context.Reviews
                      .OrderByDescending(r => r.CreatedAt)
                      .Take(count)
                      .ToListAsync();

        public async Task<List<Review>> GetReviewsCount(int count)
        {
            return await _context.Reviews
                .OrderByDescending(r => r.Id)
                .Take(count)
                .ToListAsync();
        }

        public async Task UpdateAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }
    }
}
