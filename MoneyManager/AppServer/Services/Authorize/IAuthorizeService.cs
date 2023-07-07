using AppServer.Common;
using AppServer.Models;
using AppServer.RequestModel;
using Microsoft.AspNetCore.Mvc;

namespace AppServer.Services;

public interface IAuthorizeService
{
    Task<Response> SignUp(RegisterRequest request);

    Task<TokenStorage> GenerateJWTToken(ApplicationUser user);

    Task<Response> SignIn([FromBody] LoginRequest request);
}