using System.ComponentModel.DataAnnotations;

namespace AppServer.RequestModel
{
    public class LoginRequest
    {
        [EmailAddress]
        public string? Email { get; set; }

        //public string? UserName { get; set; }

        public string Password { get; set; }
    }
}