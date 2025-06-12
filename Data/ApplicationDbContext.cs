using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RizeUp.Models;

namespace RizeUp.Data
{
    public class ApplicationDbContext : IdentityDbContext<Person>
    {
        public DbSet<Resume> Resumes { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<EndUser> EndUsers { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Project> Projects { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Resume>()
                .HasOne(r => r.EndUser)
                .WithMany(u => u.Resumes)
                .HasForeignKey(r => r.EndUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Portfolio>()
                .HasOne(p => p.EndUser)
                .WithMany(u => u.Portfolios)
                .HasForeignKey(p => p.EndUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Project>()
                .HasOne(p => p.Resume)
                .WithMany(r => r.Projects)
                .HasForeignKey(p => p.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<Project>()
                .HasOne(p => p.Portfolio)
                .WithMany(p => p.Projects)
                .HasForeignKey(p => p.PortfolioId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<Skill>()
                .HasOne(p => p.Portfolio)
                .WithMany(p => p.Skills)
                .HasForeignKey(p => p.PortfolioId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<Skill>()
                .HasOne(p => p.Resume)
                .WithMany(p => p.Skills)
                .HasForeignKey(p => p.PortfolioId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
