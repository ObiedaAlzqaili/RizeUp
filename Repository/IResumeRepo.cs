using RizeUp.Models;

namespace RizeUp.Repository
{
    public interface IResumeRepo
    {
        Task<Resume> GetResumeByIdAsync(int id);
        Task AddResumeAsync(Resume resume);
        Task UpdateResumeAsync(Resume resume);
        Task DeleteResumeAsync(int id);
        Task<IEnumerable<Resume>> GetResumesByUserIdAsync(string userId);

    }
}
