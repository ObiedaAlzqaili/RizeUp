using Microsoft.EntityFrameworkCore;
using RizeUp.Data;
using RizeUp.Models;

namespace RizeUp.Repository
{
    public class ResumeRepo : IResumeRepo
    {
        public ApplicationDbContext _DbContext { get; set; }

       

        public ResumeRepo(ApplicationDbContext dbContext)
        {
            _DbContext = dbContext;
        }
        

        

        public async Task<Resume> GetResumeByIdAsync(int id)
        {
            Resume resume = null;
            try
            {
                resume = await _DbContext.Resumes
    .Include(r => r.Educations)
    .Include(r => r.Experiences)
    .Include(r => r.Projects)
    .Include(r => r.Skills)
    .Include(r => r.Languages)
    .Include(r => r.Certificates)
    .Include(r => r.EndUser)
    .FirstOrDefaultAsync(r => r.ResumeId == id);
                if (resume == null)
                {
                    throw new KeyNotFoundException($"Resume with ID {id} not found.");
                }
                return resume;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the resume.", ex);
            }

        }

        public async Task<IEnumerable<Resume>> GetResumesByUserIdAsync(string userId)
        {
            try
            {
                var resumes = await _DbContext.Resumes.Where(r => r.EndUserId == userId).ToListAsync();
               
                return resumes;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving resumes by user ID.", ex);
            }
        }

        public async Task UpdateResumeAsync(Resume resume)
        {
            if (resume == null)
            {
                throw new ArgumentNullException(nameof(resume), "Resume cannot be null");
            }

            try
            {
                var existingResume = await _DbContext.Resumes.FindAsync(resume.EndUserId);
                if (existingResume == null)
                {
                    throw new KeyNotFoundException($"Resume with ID {resume.EndUserId} not found.");
                }
                existingResume.ModifiedDate = DateOnly.FromDateTime(DateTime.Now).ToString();
                _DbContext.Entry(existingResume).CurrentValues.SetValues(resume);
                await _DbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the resume.", ex);
            }

        }

        public async Task AddResumeAsync(Resume resume)
        {
            try
            {
                if (resume == null)
                {
                    throw new ArgumentNullException(nameof(resume), "Resume cannot be null");
                }
                await _DbContext.Resumes.AddAsync(resume);
                await _DbContext.SaveChangesAsync();


            }
            catch (ArgumentNullException ex)
            {

                throw new Exception("An error occurred while adding the resume.", ex);
            }


        }


        public async Task DeleteResumeAsync(int id)
        {
            var resume = await _DbContext.Resumes.FindAsync(id);
            if (resume == null)
            {
                throw new KeyNotFoundException($"Resume with ID {id} not found.");
            }
            //_DbContext.Resumes.Remove(resume);
            //await _DbContext.SaveChangesAsync();
        }

       
    }
}
