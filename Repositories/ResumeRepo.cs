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
                .Where(r => r.EndUserId == userId && !r.IsDeleted)
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
            var resume = await _dbContext.Resumes
                .Include(r => r.Educations)
                .Include(r => r.Experiences)
                .Include(r => r.Projects)
                .Include(r => r.Skills)
                .Include(r => r.Languages)
                .Include(r => r.Certificates)
                .FirstOrDefaultAsync(r => r.ResumeId == id);

            if (resume == null)
                throw new KeyNotFoundException($"Resume with ID {id} not found.");

            if (resume.IsDeleted)
                return; // Already soft deleted

            resume.IsDeleted = true;

        

            if (resume.Experiences != null)
                _dbContext.Experiences.RemoveRange(resume.Experiences);

            if (resume.Projects != null)
                _dbContext.Projects.RemoveRange(resume.Projects);

            if (resume.Skills != null)
                _dbContext.Skills.RemoveRange(resume.Skills);

            if (resume.Languages != null)
                _dbContext.Languages.RemoveRange(resume.Languages);

            if (resume.Certificates != null)
                _dbContext.Certificates.RemoveRange(resume.Certificates);


            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Resume>> GetResumeCount(int count)
        {
            return await _dbContext.Resumes
                .OrderByDescending(r => r.ModifiedDate ?? r.CreatedDate)
                .Take(count)
                .Include(r => r.EndUser)
                .ToListAsync();
        }
    }
}
