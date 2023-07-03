using Microsoft.AspNetCore.Identity;

namespace AppServer.Models
{
    public class UserInfo : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}