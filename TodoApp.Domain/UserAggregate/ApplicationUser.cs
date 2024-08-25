using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Identity;
using TodoApp.Domain.RefreshTokenAggregate;

namespace TodoApp.Domain.UserAggregate;


public sealed class ApplicationUser : IdentityUser
{
    private readonly List<RefreshToken> _refreshTokens = new List<RefreshToken>();

    public ApplicationUser(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();



    public void AddRefreshToken(RefreshToken refreshToken) => _refreshTokens.Add(refreshToken);
}