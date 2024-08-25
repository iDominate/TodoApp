using System.Linq.Expressions;
using System.Security.Claims;
using ErrorOr;
using TodoApp.Domain.RefreshTokenAggregate;
using TodoApp.Domain.UserAggregate;

namespace TodoApp.Application.Services;

public interface IUserRepository
{
    public Task<ApplicationUser?> GetUserAsync(Expression<Func<ApplicationUser, bool>> predicate);
    public Task<IEnumerable<Error>?> CreateAsync(ApplicationUser user, string password = "");
    public Task UpdateAsync(ApplicationUser user);
    public Task<bool> CheckPasswordsAsync(ApplicationUser user, string password);
    public Task<IEnumerable<Claim>> GetClaimsAsync(ApplicationUser user);
    public Task<IEnumerable<string>> GetRolesAsync(ApplicationUser user);

    public RefreshToken? GetRefreshToken(ApplicationUser user, string token);
}