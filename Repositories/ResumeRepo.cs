using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using RizeUp.Data;
using RizeUp.Models;

namespace RizeUp.Repository
{
    public class ResumeRepo : IResumeRepo
    {
        private readonly ApplicationDbContext _dbContext;

        public ResumeRepo(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<int> GetCountAsync()
        {
            return await _dbContext.Resumes.CountAsync();
        }

        public async Task<List<ResumeActivity>> GetRecentActivityAsync(int days)
        {
            // Calculate cutoff date
            var cutoff = DateOnly.FromDateTime(DateTime.Now.AddDays(-days));

            // Get all resumes with EndUser included
            var resumes = await _dbContext.Resumes
                .Include(r => r.EndUser)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();

            // Filter and project in-memory to avoid expression tree issues with DateOnly.TryParse
            var recentActivities = resumes
                .Where(r => DateOnly.TryParse(r.CreatedDate, out var createdDate) && createdDate >= cutoff)
                .Take(10)
                .Select(r => new ResumeActivity
                {
                    UserName = r.EndUser?.UserName ?? $"{r.EndUser?.FirstName} {r.EndUser?.LastName}".Trim(),
                    ActivityType = "Created Resume",
                    ActivityDate = DateOnly.TryParse(r.CreatedDate, out var d) ? d.ToDateTime(TimeOnly.MinValue) : DateTime.MinValue,
                    Status = "Active"
                })
                .ToList();

            return recentActivities;
        }

        public async Task<Resume> GetResumeByIdAsync(int id)
        {
            var resume = await _dbContext.Resumes
                .Include(r => r.Educations)
                .Include(r => r.Experiences)
                .Include(r => r.Projects)
                .Include(r => r.Skills)
                .Include(r => r.Languages)
                .Include(r => r.Certificates)
                .Include(r => r.EndUser)
                .FirstOrDefaultAsync(r => r.ResumeId == id);

            if (resume == null)
                throw new KeyNotFoundException($"Resume with ID {id} not found.");

            return resume;
        }

        public async Task<IEnumerable<Resume>> GetResumesByUserIdAsync(string userId)
        {
            return await _dbContext.Resumes
                .Where(r => r.EndUserId == userId)
                .ToListAsync();
        }

        public async Task UpdateResumeAsync(Resume resume)
        {
            if (resume == null) throw new ArgumentNullException(nameof(resume));

            var existingResume = await _dbContext.Resumes
                .Include(r => r.Educations)
                .Include(r => r.Experiences)
                .Include(r => r.Projects)
                .Include(r => r.Skills)
                .Include(r => r.Languages)
                .Include(r => r.Certificates)
                .FirstOrDefaultAsync(r => r.ResumeId == resume.ResumeId);

            if (existingResume == null)
                throw new KeyNotFoundException($"Resume with ID {resume.ResumeId} not found.");

            _dbContext.Entry(existingResume).CurrentValues.SetValues(resume);
            existingResume.ModifiedDate = DateOnly.FromDateTime(DateTime.Now).ToString();

            
            _dbContext.Educations.RemoveRange(existingResume.Educations ?? new List<Education>());
            existingResume.Educations = resume.Educations ?? new List<Education>();

            _dbContext.Experiences.RemoveRange(existingResume.Experiences ?? new List<Experience>());
            existingResume.Experiences = resume.Experiences ?? new List<Experience>();

            _dbContext.Projects.RemoveRange(existingResume.Projects ?? new List<Project>());
            existingResume.Projects = resume.Projects ?? new List<Project>();

            _dbContext.Skills.RemoveRange(existingResume.Skills ?? new List<Skill>());
            existingResume.Skills = resume.Skills ?? new List<Skill>();

            _dbContext.Languages.RemoveRange(existingResume.Languages ?? new List<Language>());
            existingResume.Languages = resume.Languages ?? new List<Language>();

            _dbContext.Certificates.RemoveRange(existingResume.Certificates ?? new List<Certificate>());
            existingResume.Certificates = resume.Certificates ?? new List<Certificate>();

            await _dbContext.SaveChangesAsync();
        }


        public async Task AddResumeAsync(Resume resume)
        {
            if (resume == null)
                throw new ArgumentNullException(nameof(resume), "Resume cannot be null");

            await _dbContext.Resumes.AddAsync(resume);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteResumeAsync(int id)
        {
            var resume = await _dbContext.Resumes.FindAsync(id);
            if (resume == null)
                throw new KeyNotFoundException($"Resume with ID {id} not found.");

            _dbContext.Resumes.Remove(resume);
            await _dbContext.SaveChangesAsync();
        }

        //private void UpdateChildCollection<T>(
        //    ICollection<T> existingEntities,
        //    ICollection<T> newEntities,
        //    Func<T, T, bool> keySelector) where T : class
        //{
        //    newEntities ??= new List<T>();

        //    // Remove deleted entities
        //    var toRemove = existingEntities.Where(e => !newEntities.Any(ne => keySelector(e, ne))).ToList();
        //    foreach (var entity in toRemove)
        //    {
        //        existingEntities.Remove(entity);
        //    }

        //    // Update and add entities
        //    foreach (var newEntity in newEntities)
        //    {
        //        var existingEntity = existingEntities.FirstOrDefault(e => keySelector(e, newEntity));
        //        if (existingEntity != null)
        //        {
        //            // Update fields
        //            _dbContext.Entry(existingEntity).CurrentValues.SetValues(newEntity);
        //        }
        //        else
        //        {
        //            // Add new entity
        //            existingEntities.Add(newEntity);
        //        }
        //    }
        //  }
    }
}
