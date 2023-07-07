using System.ComponentModel.DataAnnotations;

namespace AppServer.RequestModel
{
    public class TokenStorage
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}