using System.Linq.Expressions;
using System.Security.Claims;
using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Services;
using TodoApp.Domain.RefreshTokenAggregate;
using TodoApp.Domain.UserAggregate;

namespace TodoApp.Infrastructure.Authentication;

public sealed class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRepository(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> CheckPasswordsAsync(ApplicationUser user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);

    }

    public async Task<IEnumerable<Error>?> CreateAsync(ApplicationUser user, string password)
    {
        var identityResult = await _userManager.CreateAsync(user, password);
        if (!identityResult.Succeeded)
        {
            var errors = identityResult.Errors.ToList().ConvertAll(e => Error.Validation(e.Description));
            return errors;
        }
        return null;
    }

    public async Task<IEnumerable<Claim>> GetClaimsAsync(ApplicationUser user)
    {
        return await _userManager.GetClaimsAsync(user);
    }

    public RefreshToken? GetRefreshToken(ApplicationUser user, string token)
    {
        return user.RefreshTokens.FirstOrDefault(t => t.Token == token);
    }

    public async Task<IEnumerable<string>> GetRolesAsync(ApplicationUser user)
    {
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<ApplicationUser?> GetUserAsync(Expression<Func<ApplicationUser, bool>> predicate)
    {
        return await _userManager.Users.FirstOrDefaultAsync(predicate);

    }

    public async Task UpdateAsync(ApplicationUser user)
    {
        await _userManager.UpdateAsync(user);
    }
}