using Microsoft.AspNetCore.Identity;

namespace AppServer.DataModels
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public bool IsAdmin {get; set;}
    }
}