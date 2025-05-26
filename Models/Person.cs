using Microsoft.AspNetCore.Identity;

namespace RizeUp.Models
{
    public class Person : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public DateOnly DayOfBirth { get; set; }
    }
}
