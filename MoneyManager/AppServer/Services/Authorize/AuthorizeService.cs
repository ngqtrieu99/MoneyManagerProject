using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AppServer.Common;
using AppServer.Data.Repositories.Token;
using AppServer.Data.UnitOfWork;
using AppServer.Models;
using AppServer.RequestModel;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace AppServer.Services.Authorize;

public class AuthorizeService : IAuthorizeService
{
    private readonly UserManager<ApplicationUser> userManager;

    private readonly SignInManager<ApplicationUser> signInManager;

    private readonly RoleManager<IdentityRole> roleManager;

    private readonly IConfiguration configuration;

    private readonly IRefreshTokenRepository refreshTokenRepo;

    private readonly IUnitOfWork unit;

    public AuthorizeService(UserManager<ApplicationUser> userManager
        , RoleManager<IdentityRole> roleManager
        , SignInManager<ApplicationUser> signInManager
        , IConfiguration configuration
        , IRefreshTokenRepository refreshToken,
        IUnitOfWork unitOfWork
    )
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.roleManager = roleManager;
        this.configuration = configuration;
        refreshTokenRepo = refreshToken;
        unit = unitOfWork;
    }

    public async Task<Response> SignUp(RegisterRequest request)
    {
        var userExistCheck = await userManager.FindByEmailAsync(request.Email);

        if (userExistCheck != null)
        {
            return new Response
            {
                Status = false,
                Message = "User already exist. Get the fuck out"
            };
        }

        var user = new ApplicationUser()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.UserName
        };

        var response = await userManager.CreateAsync(user, request.Password);

        if (request.IsAdmin)
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }

        if (!request.IsAdmin)
        {
            await userManager.AddToRoleAsync(user, "User");
        }

        if (response.Errors.Any())
        {
            return new Response
            {
                Status = false,
                Message = "Register Failed"
            };
        }

        return new Response
        {
            Status = true,
            Message = "Register Successfully"
        };
    }

    public async Task<TokenStorage> GenerateJWTToken(ApplicationUser user)
    {
        var jwtTokenhandler = new JwtSecurityTokenHandler();
        var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));

        var userRoles = await userManager.GetRolesAsync(user);
    }

    public async Task<Response> SignIn(LoginRequest request)
    {
    }
}