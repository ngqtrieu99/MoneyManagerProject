using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AppServer.Common;
using AppServer.Data.Repositories.Token;
using AppServer.Data.UnitOfWork;
using AppServer.Models;
using AppServer.RequestModel;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        // Create the access token
        var token = new JwtSecurityToken(
            issuer: configuration["JWT:ValidIssuer"],
            audience: configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddMinutes(2),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512Signature)
        );

        var accessToken = jwtTokenhandler.WriteToken(token);
        var refreshToken = GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            JwtId = token.Id,
            UserId = user.Id,
            Token = refreshToken,
            IsUsed = false,
            IsReVoke = false,
            IssuedAt = DateTime.UtcNow,
            ExpiredAt = DateTime.UtcNow.AddHours(1)
        };

        await refreshTokenRepo.AddToken(refreshTokenEntity);
        await unit.CommitAsync();

        return new TokenStorage
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    private string GenerateRefreshToken()
    {
        var random = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(random);
            return Convert.ToBase64String(random);
        }
    }

    public async Task<Response> SignIn(LoginRequest request)
    {
        var signUserName = await userManager.FindByEmailAsync(request.Email);

        if (!signUserName.EmailConfirmed)
        {
            return new Response
            {
                Status = false,
                Message = "Your account has not been active yet"
            };
        }

        var result = await signInManager.PasswordSignInAsync(signUserName.UserName, request.Password, false, false);

        if (!result.Succeeded)
        {
            return new Response
            {
                Status = false,
                Message = "Invalid username/password",
                Data = null,
            };
        }

        //var user = await userManager.FindByEmailAsync(request.Email);
        var token = await GenerateJWTToken(signUserName);

        return new Response
        {
            Status = true,
            Message = "Authenticate Success",
            Data = token
        };
    }

    public async Task<Response> RenewToken(TokenStorage response)
    {
        var accessToken = response.AccessToken;
        var refreshToken = response.RefreshToken;
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var secretKeyBytes = Encoding.UTF8.GetBytes(configuration["JWT:Secret"]);
        var tokenValidatePagram = new TokenValidationParameters
        {
            //tu cap token
            ValidateIssuer = false,
            ValidateAudience = false,

            //ky vao token
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = false //khong kiem tra token het han
        };
        try
        {
            //check1: access token valid format
            var tokenInVerification = jwtTokenHandler.ValidateToken(accessToken, tokenValidatePagram, out var validatedToken);

            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                var result = jwtSecurityToken.Header.Alg.Equals
                    (SecurityAlgorithms.HmacSha512Signature, StringComparison.CurrentCultureIgnoreCase);
                if (!result)
                {
                    return new Response
                    {
                        Status = false,
                        Message = "invalid token"
                    };
                }
            }
            //check access token expire
            var utcExpireDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
            if (expireDate > DateTime.UtcNow)
            {
                return new Response
                {
                    Status = false,
                    Message = "Access token has not yes expire"
                };
            }
            //check 3: check refresh token exist in DB
            var storedToken = refreshTokenRepo.FindRefreshToken(refreshToken);
            if (storedToken == null)
            {
                return new Response
                {
                    Status = false,
                    Message = "refresh token does not exist"
                };
            }
            //check 4: check if refresh token is used
            if (storedToken.Result.IsUsed)
            {
                return new Response
                {
                    Status = false,
                    Message = "refresh token has been used"
                };
            }
            if (storedToken.Result.IsReVoke)
            {
                return new Response
                {
                    Status = false,
                    Message = "refresh token has been revoked"
                };
            }
            //check 5: AccessToken id = JwtId in RefreshToken
            var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            if (storedToken.Result.JwtId != jti)
            {
                return new Response
                {
                    Status = false,
                    Message = "Token does'n match"
                };
            }
            //Update token is  used
            storedToken.Result.IsReVoke = true;
            storedToken.Result.IsUsed = true;
            refreshTokenRepo.UpdateToken(storedToken.Result);
            await unit.CommitAsync();
            // create new token
            var user = await userManager.FindByIdAsync(storedToken.Result.UserId);
            var token = await GenerateJWTToken(user);
            return new Response
            {
                Status = true,
                Message = "Renew token Success",
                Data = token
            };
        }
        catch
        {
            return new Response
            {
                Status = false,
                Message = "SomeThing went wrong"
            };
        }
    }

    private DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
    {
        var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();
        return dateTimeInterval;
    }

    public async Task<Response> SignInSession([FromBody] LoginRequest request)
    {
        var signUserName = await userManager.FindByEmailAsync(request.Email);

        if (!signUserName.EmailConfirmed)
        {
            return new Response
            {
                Status = false,
                Message = "Your account has not been active yet"
            };
        }

        var result = await signInManager.PasswordSignInAsync(signUserName.UserName, request.Password, false, false);

        if (!result.Succeeded)
        {
            return new Response
            {
                Status = false,
                Message = "Invalid username/password",
                Data = null,
            };
        }
    }
}