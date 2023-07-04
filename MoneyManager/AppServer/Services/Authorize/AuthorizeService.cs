using AppServer.Common;
using AppServer.Data.Repositories.Token;
using AppServer.Data.UnitOfWork;
using AppServer.Models;
using AppServer.RequestModel;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace AppServer.Services.Authorize;

public class AuthorizeService : IAuthorizeService
{
    private readonly UserManager<ApplicationUser> userManager;

    private readonly SignInManager<ApplicationUser> signInManager;

    private readonly RoleManager<IdentityRole> roleManager;

    private readonly IConfiguration configuration;

    private readonly IRefreshTokenRepository refreshTokenRepo;

    private readonly IUnitOfWork unit;

    public AuthorizeService()
    {
    }

    // public async Task<Response> SignUp(RegisterRequest request)
    // {
    // }
}