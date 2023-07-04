using System.ComponentModel.DataAnnotations;

namespace AppServer.RequestModel
{
    public class SignInRequest
    {
        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }
    }
}