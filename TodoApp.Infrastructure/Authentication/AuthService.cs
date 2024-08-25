using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TodoApp.Application.Services;
using TodoApp.Domain.RefreshTokenAggregate;
using TodoApp.Domain.UserAggregate;

namespace TodoApp.Infrastructure.Authentication;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IUserRepository userRepository, RoleManager<IdentityRole> roleManager, IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _roleManager = roleManager;
        _jwtSettings = jwtSettings.Value;
    }

    #region Generate new token using a refresh token
    public async Task<ErrorOr<AuthModel>> GenerateTokenAsync(string token)
    {
        var existingUser = await _userRepository.GetUserAsync(u => u.RefreshTokens.Any(r => r.Token == token));
        if (existingUser is null)
        {
            return Error.Forbidden();
        }
        var currentToken = _userRepository.GetRefreshToken(existingUser, token);
        currentToken!.RevokeToken();
        await _userRepository.UpdateAsync(existingUser);
        var jwtToken = await GetJwtSecurityTokenAsync(existingUser);
        var refreshToken = GetRefreshToken();
        return new AuthModel
        {
            Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            RefreshToken = refreshToken.Token,
            Message = "Operation completed successfully",
            IsAuthenticated = true
        };
    }
    #endregion

    #region Generate new security token
    public async Task<JwtSecurityToken> GetJwtSecurityTokenAsync(ApplicationUser user)
    {
        List<Claim> userClaims = (await _userRepository.GetClaimsAsync(user)).ToList();
        List<string> userRoles = (await _userRepository.GetRolesAsync(user)).ToList();
        List<Claim> authClaims = new List<Claim>();
        userRoles.ToList().ForEach(role => authClaims.Add(new Claim("role", role)));
        List<Claim> tokenClaims = new List<Claim>{
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
        }.Union(authClaims).Union(userClaims).ToList();
        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        securityKey.KeyId = _jwtSettings.Kid;
        SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(tokenClaims),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
        };
        var token = new JwtSecurityTokenHandler().CreateJwtSecurityToken(descriptor);
        return token;

    }
    #endregion

    #region Get refresh token
    public RefreshToken GetRefreshToken()
    {
        byte[] buffer = new byte[32];
        (new Random()).NextBytes(buffer);
        return new RefreshToken(Convert.ToBase64String(buffer), DateTime.UtcNow.AddMinutes(15));
    }
    #endregion

    #region login  User Async
    public async Task<ErrorOr<AuthModel>> LoginAsync(LoginReuqest dto)
    {
        var existingUser = await this._userRepository.GetUserAsync(u => u.UserName == dto.Username);
        var isPasswordCorrect = await this._userRepository.CheckPasswordsAsync(existingUser!, dto.Password);
        if (existingUser is null || isPasswordCorrect is false)
        {
            return Error.Unauthorized("Auth.Creds.Invalid");
        }
        var jwtToken = await GetJwtSecurityTokenAsync(existingUser);
        var refreshToken = GetRefreshToken();

        return new AuthModel
        {
            IsAuthenticated = true,
            Email = existingUser.Email!,
            Roles = new List<string> { "User" },
            Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            ExpiresOn = DateTime.UtcNow.AddMinutes(15),
            RefreshToken = refreshToken.Token
        };
    }
    #endregion

    #region register User Async
    public async Task<ErrorOr<AuthModel>> RegisterAsync(RegisterRequest dto)
    {
        if (!dto.Password.Equals(dto.ConfirmPassword, StringComparison.InvariantCulture))
        {
            return Error.Unauthorized("Auth.Passwords.Failure");
        }
        var existingUser = await _userRepository.GetUserAsync(u => u.UserName == dto.UserName);
        if (existingUser is not null)
        {
            return Error.Unauthorized("Auth.User.UserExists");
        }
        existingUser = dto.ToApplicationUser();
        var result = await _userRepository.CreateAsync(existingUser, dto.Password);
        if (result is not null)
        {
            return Error.Validation(String.Join("\n", result!.ToList().ConvertAll(e => e.Description)));
        }
        var jwtToken = await GetJwtSecurityTokenAsync(existingUser);
        var refreshToken = GetRefreshToken();
        existingUser.AddRefreshToken(refreshToken);
        await _userRepository.UpdateAsync(existingUser);
        return new AuthModel
        {
            Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            RefreshToken = refreshToken.Token,
            Roles = new List<string>() { "User" },
            UserName = existingUser.UserName!,
            Email = existingUser.Email!,
            ExpiresOn = DateTime.Now.AddMinutes(15),
            IsAuthenticated = true,
            Message = "Authentication successful"
        };
    }
    #endregion
}
