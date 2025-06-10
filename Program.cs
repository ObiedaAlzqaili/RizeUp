using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.SemanticKernel;
using RizeUp.Data;
using RizeUp.Interfaces;
using RizeUp.Models;
using RizeUp.Repository;
using RizeUp.Services;


namespace RizeUp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            // Pseudocode:
            // 1. Add code to read the key from "sercrite.json".
            // 2. Register the key in the DI container as "userManagerSercirte".
            // 3. Use Configuration to load the file and access the key.

            

            var key = builder.Configuration["OpenAi:key"];

            var userSecret = builder.Configuration["key"];
             


            builder.Services.AddSingleton<Kernel>(sp =>
            {
                var kernelBuilder = Kernel.CreateBuilder();
                kernelBuilder.AddOpenAIChatCompletion("gpt-4", userSecret);
                return kernelBuilder.Build();
            });

            builder.Services.AddScoped<IResumeRepo, ResumeRepo>();
            builder.Services.AddScoped<IPortfolioRepo, PortfolioRepo>();
            builder.Services.AddSingleton<IResumeOpenAiService, ResumeOpenAiService>();
            builder.Services.AddSingleton<IPortfolioOpenAiService, PortfolioOpenAiService>();



            builder.Services.AddDefaultIdentity<Person>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

           
            builder.Services.AddControllersWithViews();

            var app = builder.Build();
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
