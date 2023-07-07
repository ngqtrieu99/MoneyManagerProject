using Microsoft.AspNetCore.Identity;

namespace AppServer.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsAdmin { get; set; }
    }
}