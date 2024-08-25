using System.IdentityModel.Tokens.Jwt;
using ErrorOr;
using TodoApp.Domain.RefreshTokenAggregate;
using TodoApp.Domain.UserAggregate;

namespace TodoApp.Application.Services;

public interface IAuthService
{
    public Task<ErrorOr<AuthModel>> RegisterAsync(RegisterRequest dto);
    public Task<JwtSecurityToken> GetJwtSecurityTokenAsync(ApplicationUser user);
    public Task<ErrorOr<AuthModel>> LoginAsync(LoginReuqest dto);
    public RefreshToken GetRefreshToken();
    public Task<ErrorOr<AuthModel>> GenerateTokenAsync(string token);
}