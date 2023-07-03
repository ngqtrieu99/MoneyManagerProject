using System.ComponentModel.DataAnnotations;

namespace AppServer.RequestModels
{
    public class SignInRequest
    {
        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }
    }
}