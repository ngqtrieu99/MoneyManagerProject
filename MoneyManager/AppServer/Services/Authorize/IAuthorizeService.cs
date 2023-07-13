using AppServer.Common;
using AppServer.Models;
using AppServer.RequestModel;
using Microsoft.AspNetCore.Mvc;

namespace AppServer.Services;

public interface IAuthorizeService
{
    Task<Response> SignUp(RegisterRequest request);

    Task<Response> SignIn([FromBody] LoginRequest request);

    Task<Response> RenewToken(TokenStorage token);

    Task<Response> SignInSession([FromBody] LoginRequest request);
}