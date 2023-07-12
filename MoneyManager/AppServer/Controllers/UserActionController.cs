using AppServer.Common;
using AppServer.RequestModel;
using AppServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Data;

namespace AppServer.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class UserActionController : ControllerBase
{
    private readonly ILogger<UserActionController> _logger;

    private readonly IAuthorizeService _authorizeService;

    public UserActionController(ILogger<UserActionController> logger, IAuthorizeService authorizeService)
    {
        _logger = logger;
        _authorizeService = authorizeService;
    }

    [HttpPost("register")]
    public async Task<Response> UserRegister([FromBody] RegisterRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return new Response
                {
                    Status = false,
                    Message = StatusCode(StatusCodes.Status400BadRequest, "Invalid Request")
                };
            }

            if (request.Password != request.ConfirmPassword)
            {
                return new Response
                {
                    Status = false,
                    Message = "Password is not matched"
                };
            }

            var response = await _authorizeService.SignUp(request);

            if (!response.Status)
            {
                return new Response
                {
                    Status = false,
                    Message = response.Message
                };
            }

            return new Response
            {
                Status = true,
                Message = response.Message
            };
        }
        catch (Exception ex)
        {
            return new Response
            {
                Status = false,
                Message = ex.Message
            };
        }
    }

    [HttpPost("login")]
    public async Task<Response> UserLogin([FromBody] LoginRequest request)
    {
        try
        {
            if (request.Email == null)
            {
                return new Response
                {
                    Status = false,
                    Message = StatusCodes.Status400BadRequest
                };
            }

            var token = await _authorizeService.SignIn(request);

            return new Response
            {
                Status = token.Status,
                Message = token.Message,
                Data = token.Data
            };
        }
        catch (Exception ex)
        {
            return new Response
            {
                Status = false,
                Message = ex.Message
            };
        }
    }

    [HttpPost("renew-token")]
    public async Task<IActionResult> RenewToken(TokenStorage model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _authorizeService.RenewToken(model);
                if (result != null)
                {
                    return Ok(result);
                }

                return Unauthorized();
            }
            else
                return BadRequest(ModelState);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("hello-token")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> HelloToken()
    {
        try
        {
            return Ok("Your token here bro");
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpGet("hello-session")]
    public async Task<IActionResult> HelloSession()
    {
        try
        {
            return Ok("Your session here bro");
        }
        catch (Exception e)
        {
            return Unauthorized(e.Message);
        }
    }
}