using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RizeUp.Models;

public static class UserManagerExtensions
{
    public static async Task<List<Person>> GetLastFiveEndUsersAsync(this UserManager<Person> userManager)
    {
        var usersInRole = await userManager.GetUsersInRoleAsync("EndUser");
        return usersInRole
            .OrderByDescending(u => u.Id)
            .Take(5)
            .ToList();
    }
}
