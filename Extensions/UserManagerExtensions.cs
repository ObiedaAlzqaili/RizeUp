using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RizeUp.Models;

public static class UserManagerExtensions
{
    public static async Task<List<Person>> GetRecentUsersAsync(this UserManager<Person> userManager, int count)
    {
        return await userManager.Users
            .OrderByDescending(u => u.Id) // Assuming Id is sequential; replace with a timestamp field if available
            .Take(count)
            .ToListAsync();
    }
}
