using AppServer.Common;
using AppServer.RequestModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AppServer.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class UserActionController : ControllerBase
{
    private readonly ILogger<UserActionController> _logger;

    public UserActionController(ILogger<UserActionController> logger)
    {
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<Response> UserRegister([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return new Response
            {
                Status = false,
                Message = StatusCode(StatusCodes.Status400BadRequest, "Invalid Request")
            };
        }
        
        
        
        
    }
    
}