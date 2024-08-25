

namespace TodoApp.Domain.RefreshTokenAggregate;

public sealed class RefreshToken
{
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiresOn { get; private set; }
    public bool HasExpired => DateTime.Now.ToLocalTime() >= ExpiresOn;
    public DateTime? RevokedOn { get; private set; } = null;
    public DateTime CreatedOn { get; private set; } = DateTime.UtcNow;
    public bool IsActive => RevokedOn is not null && HasExpired is true;

    public RefreshToken(string token, DateTime expiresOn)
    {
        Token = token;
        ExpiresOn = expiresOn;
    }

    public void RevokeToken()
    {
        this.RevokedOn = DateTime.UtcNow;
    }

}